import { useLocalStorage } from '@mantine/hooks';
import {useState} from "react";

export function useHiddenAccounts() {
    const [hiddenIds, setHiddenIds] = useState<string[]>(() => {
        try {
            const raw = localStorage.getItem('hidden-accounts');
            return raw ? JSON.parse(raw) as string[] : [];
        } catch {
            return [];
        }
    });

    const toggleHidden = (id: string) => {
        setHiddenIds(prev => {
            const next = prev.includes(id)
                ? prev.filter(x => x !== id)
                : [...prev, id];

            try {
                localStorage.setItem('hidden-accounts', JSON.stringify(next));
            } catch {
            }

            return next;
        });
    };

    return { hiddenIds, toggleHidden };
}