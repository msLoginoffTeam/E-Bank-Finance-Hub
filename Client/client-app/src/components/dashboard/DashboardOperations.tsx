import { Card, Button, Title } from '@mantine/core';
import { OperationList } from '../operations/OperationList';
import { Link } from 'react-router-dom';

export const DashboardOperations = ({ operations }: { operations: any[] }) => {
    return (
        <Card shadow="lg" p="md">
            <Title order={3} mb="sm">История операций</Title>
            <OperationList operations={operations} />
            {operations ? <Button disabled={!operations} fullWidth mt="md" component={Link} to="/operations">
                Все операции
            </Button> : undefined}
        </Card>
    );
};
