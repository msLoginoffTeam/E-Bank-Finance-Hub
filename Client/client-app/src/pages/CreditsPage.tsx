import { Container, Title, SimpleGrid, Button, Stack } from '@mantine/core';
import { useCreditsQuery } from '../queries/credits.queries';
import { useNavigate } from 'react-router-dom';
import { CreditCard } from '../components/credits/CreditCard';
import { useState } from 'react';
import {OpenCreditModal} from "../modals/OpenCreditModal.tsx";
import {CreditDetails} from "../types/creditTypes.ts";

export const CreditsPage = () => {
    const { data: credits, isLoading } = useCreditsQuery();
    const [openModal, setOpenModal] = useState(false);
    const navigate = useNavigate();

    if (isLoading) return <p>Загрузка...</p>;

    const order: { [key in CreditDetails["status"]]: number } = {
        "Expired": 0,
        "Open": 1,
        "Closed": 2,
    };

    return (
        <Container size="md" py="xl">
            <Stack>
                <Title>Мои кредиты</Title>
                <Button onClick={() => setOpenModal(true)}>Оформить новый кредит</Button>
                <SimpleGrid cols={2}>
                    {credits?.sort((a, b) => order[a.status] - order[b.status]).map((credit) => (
                        <CreditCard hasOpenButton={true} key={credit.id} credit={credit} onPayOff={() => navigate(`/credits/${credit.id}`)} />
                    ))}
                </SimpleGrid>
            </Stack>
            {openModal && <OpenCreditModal onClose={() => setOpenModal(false)} />}
        </Container>
    );
};
