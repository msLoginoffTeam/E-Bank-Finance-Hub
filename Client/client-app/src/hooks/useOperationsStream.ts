import { useEffect, useState } from 'react';

interface RawOp {
    Id?: string;
    id?: string;
    Amount?: number;
    amount?: number;
    OperationType?: string;
    operationType?: string;
    Time?: string;
    time?: string;
}

export function useOperationsStream(accountId: string) {
    const [operations, setOperations] = useState<RawOp[]>([]);
    const accessToken = localStorage.getItem('token');

    useEffect(() => {
        if (!accountId || !accessToken) return;

        setOperations([]);

        const url = `ws://localhost:5009/token=${accessToken}&accountId=${accountId}`;
        const ws  = new WebSocket(url);

        ws.onopen = () => console.log('WS открыт:', url);
        ws.onerror = e => console.error('WS ошибка', e);
        ws.onclose = () => console.log('WS закрыт');

        ws.onmessage = ev => {
            let parsed: any;
            try {
                parsed = JSON.parse(ev.data);
            } catch {
                console.warn('Невалидный WS payload', ev.data);
                return;
            }

            if (Array.isArray(parsed)) {
                const allOps = parsed.map(m => ({
                    id:            m.Id   ?? m.id,
                    amount:        m.Amount ?? m.amount,
                    operationType: m.OperationType ?? m.operationType,
                    time:          m.Time  ?? m.time,
                }));
                allOps.sort((a, b) => new Date(b.time).getTime() - new Date(a.time).getTime());
                setOperations(allOps);
            }
            else {
                const m = parsed as RawOp;
                const op = {
                    id:            m.Id   ?? m.id,
                    amount:        m.Amount ?? m.amount,
                    operationType: m.OperationType ?? m.operationType,
                    time:          m.Time  ?? m.time,
                };
                setOperations(prev => [op, ...prev]);
            }
        };

        return () => {
            ws.close();
        };
    }, [accountId, accessToken]);

    return { operations };
}
