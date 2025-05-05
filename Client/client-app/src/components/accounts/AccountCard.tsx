import {Card, Text, Button, Group, Modal, CloseIcon, ActionIcon} from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {ModalTransaction} from "../../modals/ModalTransaction.tsx";
import {useCloseAccountMutation} from "../../queries/accounts.queries.ts";
import {currencyData, CurrencyEnum} from "../../types/currency.ts";

export const AccountCard = ({ account, isHidden, onToggleHidden }: { account: any; isHidden?: boolean,
    onToggleHidden?: () => void, isDashboard?: boolean }) => {
    const [isModalOpen, setModalOpen] = useState(false);
    const [transactionType, setTransactionType] = useState<'deposit' | 'withdraw'>('deposit');
    const navigate = useNavigate();
    const closeAccountMutation = useCloseAccountMutation(account.id);
    const [isCloseModalOpen, setIsCloseModalOpen] = useState(false);

    const handleOpenModal = (type: 'deposit' | 'withdraw') => {
        setTransactionType(type);
        setModalOpen(true);
    };

    const handleCloseAccount = async () => {
        await closeAccountMutation.mutateAsync(account.id);
        setIsCloseModalOpen(false);
    };

    const disabled = isHidden;

    let currency;
    if (account.currency === currencyData[CurrencyEnum.Euro]) {
        currency = '€';
    }
    else if (account.currency === currencyData[CurrencyEnum.Ruble]) {
        currency = '₽'
    }
    else {
        currency = '$'
    }

    return (
        <Card mt={10} mb={10} shadow={'xs'} withBorder p="md" radius="lg" style={{ opacity: !account.isClosed ? 1 : 0.5 }}>

            <ActionIcon
                variant="subtle"
                size="lg"
                onClick={onToggleHidden}
                style={{ position: 'absolute', top: 8, right: 8 }}
            >
                {isHidden ? <CloseIcon /> : <CloseIcon />}
            </ActionIcon>

            <Group mt="xs">
                <Text size="lg" style={{ flex: 1 }}>
                    Баланс: {account.balance} {currency}
                </Text>
            </Group>

            <Text size="sm" color="dimmed">{account.name}</Text>
            <Text size="sm" color="dimmed">Счет №{account.id}</Text>
            {!account.isClosed && (
                <Group grow mt="md">
                    <Button color="green" onClick={() => handleOpenModal('deposit')}>Пополнить</Button>
                    <Button color="red" onClick={() => handleOpenModal('withdraw')}>Снять</Button>
                </Group>
            )}

            <Button variant="subtle" fullWidth mt="sm" onClick={() => navigate(`/accounts/${account.id}`)}>
                Открыть
            </Button>

            <ModalTransaction
                opened={isModalOpen}
                onClose={() => setModalOpen(false)}
                account={account}
                transactionType={transactionType}
            />

            {!account.isClosed ? (
                <Group grow mt="md">
                    <Button color="red" onClick={() => setIsCloseModalOpen(true)}>Закрыть счет</Button>
                </Group>
            ) : (
                <Text color="red" size="sm">Счет закрыт</Text>
            )}

            <Modal opened={isCloseModalOpen} onClose={() => setIsCloseModalOpen(false)} title="Подтвердите действие">
                <Text>Вы уверены, что хотите закрыть этот счет?</Text>
                <Group mt="md">
                    <Button color="red" onClick={handleCloseAccount} loading={closeAccountMutation.isPending}>
                        Закрыть
                    </Button>
                    <Button variant="outline" onClick={() => setIsCloseModalOpen(false)}>Отмена</Button>
                </Group>
            </Modal>
        </Card>
    );
};
