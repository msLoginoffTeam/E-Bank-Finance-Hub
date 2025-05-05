import { useEffect, useState } from 'react';

interface Operation {
    id: string;
    amount: number;
    operationType: string;
    time: string;
}

export function useOperationsStream(accountId: string) {
    const [operations, setOperations] = useState<Operation[]>([]);
    const accessToken = localStorage.getItem('token');

    useEffect(() => {
        if (!accountId || !accessToken) {
            return;
        }

        setOperations([]);

        const url = `ws://localhost:5009/token=${accessToken}&accountId=${accountId}`;
        const ws  = new WebSocket(url);

        ws.onopen = () => console.log('WS открыт:', url);
        ws.onerror = e => console.error('WS ошибка', e);
        ws.onclose = () => console.log('WS закрыт');

        ws.onmessage = ev => {
            try {
                const parsed = JSON.parse(ev.data);
                const msgs   = Array.isArray(parsed) ? parsed : [parsed];
                const newOps = msgs.map(m => ({
                    id:            m.Id   || m.id,
                    amount:        m.Amount ?? m.amount,
                    operationType: m.OperationType ?? m.operationType,
                    time:          m.Time  ?? m.time,
                }));
                setOperations(prev => [...prev, ...newOps]);
            } catch {
                console.warn('Невалидный WS payload', ev.data);
            }
        };

        return () => {
            ws.close();
        };
    }, [accountId, accessToken]);

    return { operations };
}
