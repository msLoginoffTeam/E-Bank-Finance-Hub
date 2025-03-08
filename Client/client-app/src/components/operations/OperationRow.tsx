import { Table } from '@mantine/core';

export const OperationRow = ({ operation }: { operation: any }) => {
    return (
        <Table.Tr>
            <Table.Td>{operation.operationType}</Table.Td>
            <Table.Td>{operation.amountInRubles}р.</Table.Td>
        </Table.Tr>
    );
};
