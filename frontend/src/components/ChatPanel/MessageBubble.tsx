'use client'

import { useState, useRef, useEffect } from 'react'
import { Message } from '@/types'
import StatusTick from '@/components/UI/StatusTick'
import ReactionPicker from './ReactionPicker'
import AudioMessage from './AudioMessage'
import ImageMessage from './ImageMessage'
import { format } from 'date-fns'
import api from '@/services/api'
import { useAuthStore } from '@/store/authStore'
import { useConversationStore } from '@/store/conversationStore'

interface Props {
  message: Message
  conversationId: string
}

export default function MessageBubble({ message, conversationId }: Props) {
  const agent = useAuthStore((s) => s.agent)
  const addReaction = useConversationStore((s) => s.addReaction)
  const [showPicker, setShowPicker] = useState(false)
  const [pickerPos, setPickerPos] = useState<'top' | 'bottom'>('top')
  const bubbleRef = useRef<HTMLDivElement>(null)

  const isOut = message.isFromAgent
  const time = format(new Date(message.createdAt), 'HH:mm')

  const reactionGroups = message.reactions.reduce<Record<string, number>>(
    (acc, r) => {
      acc[r.emoji] = (acc[r.emoji] ?? 0) + 1
      return acc
    },
    {}
  )

  function handleContextMenu(e: React.MouseEvent) {
    e.preventDefault()

    const rect = bubbleRef.current?.getBoundingClientRect()
    if (rect) {
      setPickerPos(rect.top > 150 ? 'top' : 'bottom')
    }
    setShowPicker(true)
  }

  async function handleReact(emoji: string) {
    setShowPicker(false)
    try {
      await api.post(
        `/api/conversation/${conversationId}/messages/${message.id}/react`,
        { emoji }
      )
      addReaction(message.id, emoji, agent?.id ?? 'agent', true)
    } catch (err) {
      console.error('Failed to send reaction', err)
    }
  }

  useEffect(() => {
    function handleClick() { setShowPicker(false) }
    if (showPicker) document.addEventListener('click', handleClick)
    return ()  => document.removeEventListener('click', handleClick)
  }, [showPicker])

  return (
    <div className={`flex ${isOut ? 'justify-end' : 'justify-start'} group`}>
      <div className="relative max-w-[62%]">

        {showPicker && (
          <div className={`absolute z-10 ${isOut ? 'right-0' : 'left-0'}
            ${pickerPos === 'top' ? 'bottom-full mb-1' : 'top-full mt-1'}`}
          >
            <ReactionPicker onReact={handleReact} />
          </div>
        )}

        <div
          ref={bubbleRef}
          onContextMenu={handleContextMenu}
          className={`
            relative px-3 pt-2 pb-1 rounded-lg cursor-pointer
            transition-all select-none
            ${isOut
              ? 'bg-wa-bubble_out rounded-tr-none'
              : 'bg-wa-bubble_in rounded-tl-none'
            }
          `}
        >
          {isOut && message.sentByAgentName && (
            <p className="text-wa-teal text-xs font-medium mb-1">
              {message.sentByAgentName}
            </p>
          )}

          {message.type === 'Audio' ? (
            <AudioMessage message={message} />
          ) : message.type === 'Image' ? (
            <ImageMessage message={message} />
          ) : message.type === 'Document' ? (
            <DocumentMessage message={message} />
          ) : (
            <p className="text-wa-text text-sm leading-relaxed whitespace-pre-wrap wrap-break-word">
              {message.content}
            </p>
          )}

          <div className="flex items-center justify-end gap-1 mt-1">
            <span className="text-[11px] text-wa-muted">{time}</span>
            {isOut && <StatusTick status={message.status} />}
          </div>
        </div>

        {Object.keys(reactionGroups).length > 0 && (
          <div className={`flex flex-wrap gap-1 mt-1
            ${isOut ? 'justify-end' : 'justify-start'}`}
          >
            {Object.entries(reactionGroups).map(([emoji, count]) => (
              <button
                key={emoji}
                onClick={() => handleReact(emoji)}
                className="
                  flex items-center gap-1 bg-wa-panel border border-wa-border
                  rounded-full px-2 py-0.5 text-xs hover:bg-wa-hover
                  transition-colors
                "
              >
                <span>{emoji}</span>
                {count > 1 && (
                  <span className="text-wa-muted">{count}</span>
                )}
              </button>
            ))}
          </div>
        )}

      </div>
    </div>
  )
}

function DocumentMessage({ message }: { message: Message }) {
  return (
    <a
      href={message.mediaUrl}
      target="_blank"
      rel="noopener noreferrer"
      className="flex items-center gap-2 text-wa-text hover:underline"
    >
      <span className="text-2xl">📄</span>
      <span className="text-sm truncate max-w-40">
        {message.content || 'Document'}
      </span>
    </a>
  )
}