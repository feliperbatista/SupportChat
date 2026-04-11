import { format } from 'date-fns';
import { toZonedTime } from 'date-fns-tz';

export const formatBrazilTime = (date: string) => {
  const safeDate = date.endsWith('Z') ? date : date + 'Z';

  const zonedDate = toZonedTime(safeDate, 'America/Sao_Paulo');

  return format(zonedDate, 'HH:mm');
};