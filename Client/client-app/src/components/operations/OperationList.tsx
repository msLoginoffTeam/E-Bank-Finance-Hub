import {Table, Text} from '@mantine/core';
import { OperationRow } from './OperationRow';

export const OperationList = ({ operations, limit }: { operations: any[], limit?: number }) => {
    return (
        <Table>
            <Table.Thead>
                <Table.Tr>
                    <Table.Th>Тип</Table.Th>
                    <Table.Th>Сумма</Table.Th>
                </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
                {operations ? operations.slice(0, limit).map(op => <OperationRow key={op.time} operation={op} />) : <Text >Нет операций</Text>}
            </Table.Tbody>
        </Table>
    );
};
