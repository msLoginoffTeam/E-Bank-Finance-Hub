import { Container, Title, Stack } from '@mantine/core';
import {
    usePayOffCreditMutation,
    useCreditDetailsQuery
} from '../queries/credits.queries';
import { useParams } from 'react-router-dom';
import { CreditCard } from '../components/credits/CreditCard';
import { CreditHistoryList } from '../components/credits/CreditHistoryList';

export const CreditDetailsPage = () => {
    const { creditId } = useParams();
    const { data: credit, isLoading } = useCreditDetailsQuery(creditId!);
    const { mutate: payOffCredit } = usePayOffCreditMutation();

    if (isLoading) return <p>Загрузка...</p>;

    return (
        <Container size="md" py="xl">
            <Stack>
                <Title>Детали кредита</Title>
                {credit && <CreditCard credit={credit} onPayOff={() => payOffCredit({ creditId: credit.id, accountId: credit.id, amount: credit.amount })} />}
                <CreditHistoryList credit={credit} />
            </Stack>
        </Container>
    );
};
