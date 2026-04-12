export interface FilePreview {
  file: File;
  url: string;
  type: 'image' | 'video' | 'audio' | 'document';
}

export function getFileType(file: File): FilePreview['type'] {
  if (file.type.startsWith('image/')) return 'image';
  if (file.type.startsWith('video/')) return 'video';
  if (file.type.startsWith('audio/')) return 'audio';
  return 'document';
}

export function formatBytes(bytes: number) {
  if (bytes < 1024) return `${bytes} B`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export function getMessageType(fileType: FilePreview['type']) {
  switch (fileType) {
    case 'image':
      return 'Image';
    case 'video':
      return 'Video';
    case 'audio':
      return 'Audio';
    case 'document':
      return 'Document';
  }
}
