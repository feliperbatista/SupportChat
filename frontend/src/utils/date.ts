import { format, isToday, isYesterday } from 'date-fns';
import { toZonedTime } from 'date-fns-tz';

export const formatBrazilTime = (date: string) => {
  const safeDate = date.endsWith('Z') ? date : date + 'Z';

  const zonedDate = toZonedTime(safeDate, 'America/Sao_Paulo');

  return format(zonedDate, 'HH:mm');
};

export const formatBrazilDate = (date: string) => {
  const safeDate = date.endsWith('Z') ? date : date + 'Z';

  const zonedDate = toZonedTime(safeDate, 'America/Sao_Paulo');

  if (isToday(zonedDate)) return format(zonedDate, 'HH:mm');
  if (isYesterday(zonedDate)) return 'Yesterday';
  return format(zonedDate, 'dd/MM/yyyy');
};
