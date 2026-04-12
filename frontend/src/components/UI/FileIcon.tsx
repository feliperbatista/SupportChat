import { FilePreview } from "@/utils/file"
import { FileText, Film, ImageIcon, Music } from "lucide-react"

export default function FileIcon({type}: {type: FilePreview['type']}) {
  const cls = 'w-6 h-6'
    switch(type) {
        case 'image': return <ImageIcon className={cls}/>
        case 'video': return <Film className={cls}/>
        case 'audio': return <Music className={cls}/>
        case 'document': return <FileText className={cls}/>
    }
}