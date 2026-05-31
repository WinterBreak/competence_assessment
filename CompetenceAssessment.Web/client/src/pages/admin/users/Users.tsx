import React, {useEffect, useMemo, useState} from 'react';
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
    TableToolbarAction,
    TableToolbarSearch,
    Loading,
    ToastNotification
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { Edit } from '@carbon/react/icons';
import { userStore } from './stores/userStore';
// import {UserModal} from "../users/components/UserModal";
import {User} from "./types/user.types";
import {competenceModelStore} from "../competendeModels/stores/competenceModelStore";
// import {CreateUserDto, UpdateUserDto} from "../users/types/user.types";

export const Users: React.FC = observer(() => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingUser, setEditingUser] = useState<User | null>(null);

    useEffect(() => {
        userStore.loadUsers();
    }, []);

    const handleEdit = (id: string) => {
        let user = userStore.users.find(c => c.id.toString() === id) ?? null;
        setEditingUser(user);
        setIsModalOpen(true);
    };

    // const handleModalSubmit = async (data: { id?: string; text: string; type: string; answer?: string | null }) => {
    //     if (editingUser) {
    //         let editDto: UpdateUserDto = {
    //             id: Number(data.id),
    //             text: data.text,
    //             type: Number(data.type),
    //             answer: data.answer ?? ''
    //         };
    //         await userStore.updateUser(editDto);
    //     } else {
    //         let createDto: CreateUserDto = {
    //             text: data.text,
    //             type: Number(data.type),
    //             answer: data.answer ?? ''
    //         };
    //         await userStore.createUser(createDto);
    //     }
    //     setIsModalOpen(false);
    //     setEditingUser(null);
    // };
    //
    // const handleModalClose = () => {
    //     setIsModalOpen(false);
    //     setEditingUser(null);
    // };

    // const handleExport = async () => {
    //     await userStore.exportUsers();
    // };

    const rows = useMemo(() =>
            userStore.users.map(t => ({
                id: String(t.id),
                fullName: `${t.lastName} ${t.firstName} ${t.secondName}`,
                email: t.email,
                position: t.position,
                department: t.department,
                bossName: t.bossName,
            })),
        [userStore.users]);

    if (userStore.isLoading && userStore.users.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }
    
    const headers = [
        { key: 'fullName', header: 'ФИО' },
        { key: 'email', header: 'Почта' },
        { key: 'position', header: 'Должность' },
        { key: 'department', header: 'Подразделение' },
        { key: 'bossName', header: 'Руководитель' },
    ];
    
    return (
        <div>
            {userStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={userStore.error}
                    onClose={() => userStore.clearError()}
                />
            )}

            <DataTable rows={rows} headers={headers}>
                {({
                      rows,
                      headers,
                      selectedRows,
                      getTableProps,
                      getHeaderProps,
                      getRowProps,
                      getSelectionProps,
                      getTableContainerProps,
                      getToolbarProps,
                      onInputChange,
                  }) => {

                    const selectedIds = selectedRows.map(r => r.id);

                    return (
                        <TableContainer
                            title="Задания"
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            userStore.setSearchTerm(e.toString());
                                        }}
                                    />
                                    
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
                                                            <Button
                                                                kind="ghost"
                                                                size="sm"
                                                                iconDescription="Редактировать"
                                                                onClick={() => handleEdit(row.id)}
                                                                renderIcon={Edit}
                                                                hasIconOnly
                                                            />
                                                        </TableCell>
                                                    );
                                                }

                                                return (
                                                    <TableCell key={cell.id}>
                                                        {cell.value}
                                                    </TableCell>
                                                );
                                            })}
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    );
                }}
            </DataTable>

            <Pagination
                page={userStore.currentPage}
                pageSize={userStore.pageSize}
                totalItems={userStore.totalItems}
                backwardText="Назад"
                forwardText="Вперед"
                itemRangeText={(min, max, total) => `${ min }–${ max } из ${ total } элементов`}
                itemsPerPageText="Элементов на странице"
                pageRangeText={(_current, total) => `из ${ total } ${ total === 1 ? 'страницы' : 'страниц' }`}
                pageSizes={[10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== userStore.pageSize) {
                        userStore.setPageSize(pageSize);
                    } else {
                        userStore.setCurrentPage(page);
                    }
                }}
            />

            {/*<UserModal*/}
            {/*    isOpen={isModalOpen}*/}
            {/*    onClose={handleModalClose}*/}
            {/*    onSubmit={handleModalSubmit}*/}
            {/*    initialData={editingUser || undefined}*/}
            {/*    mode={editingUser ? 'edit' : 'create'}*/}
            {/*/>*/}
        </div>
    );
});