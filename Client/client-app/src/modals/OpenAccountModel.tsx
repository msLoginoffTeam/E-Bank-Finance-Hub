import { Modal, Button, TextInput } from '@mantine/core';
import { useState } from 'react';
import { useOpenAccountMutation } from '../queries/accounts.queries';

export const OpenAccountModal = ({ opened, onClose }: { opened: boolean; onClose: () => void }) => {
    const [accountName, setAccountName] = useState('');
    const openAccountMutation = useOpenAccountMutation(accountName);

    const handleOpenAccount = async () => {
        if (!accountName.trim()) return;
        await openAccountMutation.mutateAsync();
        onClose();
    };

    return (
        <Modal opened={opened} onClose={onClose} title="Открытие нового счета">
            <TextInput
                label="Название счета"
                placeholder="Введите название"
                value={accountName}
                onChange={(e) => setAccountName(e.currentTarget.value)}
            />
            <Button fullWidth mt="md" onClick={handleOpenAccount} loading={openAccountMutation.isPending}>
                Открыть счет
            </Button>
        </Modal>
    );
};
