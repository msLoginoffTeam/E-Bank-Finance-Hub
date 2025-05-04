import {Button, Modal, NativeSelect, TextInput} from '@mantine/core';
import {useState} from 'react';
import {useOpenAccountMutation} from '../queries/accounts.queries';
import {currencyData, CurrencyEnum} from "../types/currency.ts";

export const OpenAccountModal = ({ opened, onClose }: { opened: boolean; onClose: () => void }) => {
    const [accountName, setAccountName] = useState('');
    const [currency, setCurrency] = useState<string>(currencyData[CurrencyEnum.Ruble])
    const openAccountMutation = useOpenAccountMutation(accountName, currency);

    const handleOpenAccount = async () => {
        if (!accountName.trim()) return;
        await openAccountMutation.mutateAsync();
        onClose();
    };

    const select = (
        <NativeSelect
            data={Object.keys(currencyData)}
            rightSectionWidth={28}
            onChange={(value) => setCurrency(currencyData[value.currentTarget.value])}
            styles={{
                input: {
                    fontWeight: 500,
                    borderTopLeftRadius: 0,
                    borderBottomLeftRadius: 0,
                    width: 92,
                    marginRight: -2,
                },
            }}
        />
    );

    return (
        <Modal opened={opened} onClose={onClose} title="Открытие нового счета">
            {/*<TextInput*/}
            {/*    label="Название счета"*/}
            {/*    placeholder="Введите название"*/}
            {/*    value={accountName}*/}
            {/*    onChange={(e) => setAccountName(e.currentTarget.value)}*/}
            {/*/>*/}
            <TextInput
                label="Название счета"
                placeholder="Введите название"
                value={accountName}
                onChange={(e) => {
                    setAccountName(e.currentTarget.value)
                }}
                rightSection={select}
                rightSectionWidth={92}
            />
            <Button fullWidth mt="md" onClick={handleOpenAccount} loading={openAccountMutation.isPending}>
                Открыть счет
            </Button>
        </Modal>
    );
};
