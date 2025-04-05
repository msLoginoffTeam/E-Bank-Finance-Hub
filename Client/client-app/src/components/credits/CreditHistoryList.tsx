import { Table } from '@mantine/core';

export const CreditHistoryList = (credit: any) => {
    return (
        <Table>
            <thead>
            <tr style={{textAlign: "left" }}>
                <th>Дата</th>
                <th>Сумма</th>
                <th>Тип</th>
            </tr>
            </thead>
            <tbody>
            {credit.credit.paymentHistory?.map((entry: any) => (
                <tr key={entry.id}>
                    <td>{new Date(entry.paymentDate).toLocaleDateString()}</td>
                    <td>{entry.paymentAmount} ₽</td>
                    <td>{entry.type}</td>
                </tr>
            ))}
            </tbody>
        </Table>
    );
};
