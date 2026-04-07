import * as signalR from '@microsoft/signalr';

let connection: signalR.HubConnection | null = null;

export function getConnection(): signalR.HubConnection {
  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl(process.env.NEXT_PUBLIC_HUB_URL!, {
        accessTokenFactory: () => {
          return localStorage.getItem('token') ?? '';
        },
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();
  }
  return connection;
}

export async function startConnection(): Promise<void> {
  const conn = getConnection();
  if (conn.state === signalR.HubConnectionState.Disconnected) {
    await conn.start();
    console.log('[SignalR] connected');
  }
}

export async function stopConnection(): Promise<void> {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    await connection.stop();
    console.log('[SignalR] disconnected');
  }
}
