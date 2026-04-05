import React, { useEffect, useState, useMemo } from 'react';
import {
    DataTable,
    Table,
    TableHead,
    TableRow,
    TableHeader,
    TableBody,
    TableCell,
    TableSelectAll,
    TableSelectRow,
    Button,
    Pagination,
    TableContainer,
    TableToolbar,
    TableToolbarContent,
    TableToolbarMenu,
    TableToolbarSearch
} from '@carbon/react';
import { Edit } from '@carbon/react/icons';

interface User {
    id: string;
    name: string;
    email: string;
}

const data: User[] = [
    { id: "1", name: "Alice", email: "email" },
    { id: "2", name: "Bob", email: "email" }
]

export const Users: React.FC = () => {
    const [users, setUsers] = useState<User[]>([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(5);

    const filteredData = useMemo(() => {
        if (!searchTerm) return users;

        return users.filter(item =>
            item.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            item.email.toLowerCase().includes(searchTerm.toLowerCase())
        );
    }, [users, searchTerm]);

    const paginatedData = useMemo(() => {
        const startIndex = (currentPage - 1) * pageSize;
        return filteredData.slice(startIndex, startIndex + pageSize);
    }, [filteredData, currentPage, pageSize]);

    useEffect(() => {
        setCurrentPage(1);
    }, [searchTerm]);

    useEffect(() => {
        setUsers(data);
    }, []);

    const handleEdit = (id: string) => {
        console.log('Edit:', id);
    };

    const handleUpdate = () => {
        console.log('Export');
    };

    return (
        <div>
            <DataTable rows={paginatedData} headers={[
                { key: 'name', header: 'ФИО' },
                { key: 'email', header: 'Почта' }
            ]}>
                {({
                      rows,
                      headers,
                      getTableProps,
                      getHeaderProps,
                      getRowProps,
                      getSelectionProps,
                      getTableContainerProps,
                      getToolbarProps,
                      onInputChange,
                      selectedRows,
                  }) => (
                    <TableContainer
                        title="Пользователи"
                        description={`Всего: ${filteredData.length} пользователей`}
                        {...getTableContainerProps()}
                    >
                        <TableToolbar {...getToolbarProps()}>
                            <TableToolbarContent>
                                <TableToolbarSearch
                                    onChange={(e) => {
                                        onInputChange(e);
                                        setSearchTerm(e.toString); //  TODO тут не будет работать, вероятно
                                    }}
                                    placeholder="Поиск..."
                                />
                                <TableToolbarMenu>
                                </TableToolbarMenu>
                                <Button kind="primary" onClick={handleUpdate}>
                                    Обновить данные
                                </Button>
                            </TableToolbarContent>
                        </TableToolbar>

                        <Table {...getTableProps()}>
                            <TableHead>
                                <TableRow>
                                    <TableSelectAll {...getSelectionProps()} />
                                    {headers.map(header => (
                                        <TableHeader {...getHeaderProps({ header })} key={header.key}>
                                            {header.header}
                                        </TableHeader>
                                    ))}
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {rows.map(row => (
                                    <TableRow {...getRowProps({ row })} key={row.id}>
                                        <TableSelectRow {...getSelectionProps({ row })} />
                                        {row.cells.map(cell => {
                                            if (cell.info.header === 'actions') {
                                                return (
                                                    <TableCell key={cell.id}>
                                                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                                                            <Button
                                                                kind="ghost"
                                                                size="sm"
                                                                onClick={() => handleEdit(row.id)}
                                                                renderIcon={Edit}
                                                                hasIconOnly
                                                            />
                                                        </div>
                                                    </TableCell>
                                                );
                                            }
                                            return <TableCell key={cell.id}>{cell.value}</TableCell>;
                                        })}
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>

                        {selectedRows.length > 0 && (
                            <div style={{ padding: '1rem', background: '#e8f2ff' }}>
                                Выбрано: {selectedRows.length}
                            </div>
                        )}
                    </TableContainer>
                )}
            </DataTable>

            <Pagination
                backwardText="Назад"
                forwardText="Вперед"
                itemsPerPageText="Строк на странице:"
                page={currentPage}
                pageSize={pageSize}
                pageSizes={[20, 50, 100]}
                totalItems={filteredData.length}
                onChange={({ page, pageSize }) => {
                    setCurrentPage(page);
                    setPageSize(pageSize);
                }}
                pageRangeText={(current, total) => `${current} из ${total}`}
                itemRangeText={(min, max, total) => `${min}-${max} из ${total}`}
            />
        </div>
    );
};