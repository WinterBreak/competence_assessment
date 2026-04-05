import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { DataTable, Table, TableHead, TableRow, TableHeader, TableBody, TableCell } from '@carbon/react';

interface User {
    id: string;
    name: string;
    email: string;
}

const data: User[] = [
    { id: "1", name: "Alice", email: "email" },
    { id: "2", name: "Bob", email: "email" }
    ]

export const StartAssessment: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);

    useEffect(() => {
        setUsers(data);
    }, []);

    // const fetchUsers = async () => {
    //     // Пример: const response = await api.get('/users');
    //     // setUsers(response.data);
    // };

    return (
        <div>
            <h1>Начать оценку</h1>
            <DataTable rows={users} headers={[
                { key: 'id', header: 'ID' },
                { key: 'name', header: 'Name' },
                { key: 'email', header: 'Email' }
            ]}>
                {({ rows, headers, getTableProps }) => (
                    <Table {...getTableProps()}>
                        <TableHead>
                            <TableRow>
                                {headers.map(header => (
                                    <TableHeader key={header.key}>
                                        {header.header}
                                    </TableHeader>
                                ))}
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {rows.map(row => (
                                <TableRow key={row.id}>
                                    {row.cells.map(cell => (
                                        <TableCell key={cell.id}>
                                            <Link to={`/users/${row.id}`}>{cell.value}</Link>
                                        </TableCell>
                                    ))}
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                )}
            </DataTable>
        </div>
    );
};