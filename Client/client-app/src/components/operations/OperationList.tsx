import {Table, Text} from '@mantine/core';
import { OperationRow } from './OperationRow';

export const OperationList = ({ operations }: { operations: any[] }) => {
    console.log(operations);
    return (
        <Table>
            <Table.Thead>
                <Table.Tr>
                    <Table.Th>Тип</Table.Th>
                    <Table.Th>Сумма</Table.Th>
                </Table.Tr>
            </Table.Thead>
            <Table.Tbody>
                {operations ? operations.map(op => <OperationRow key={op.id} operation={op} />) : <Text >Нет операций</Text>}
            </Table.Tbody>
        </Table>
    );
};
