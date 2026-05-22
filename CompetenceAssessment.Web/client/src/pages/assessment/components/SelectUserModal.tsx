// src/components/AssessmentCreateModal/SelectUsersModal.tsx
import React, { useState, useEffect, useMemo } from 'react';
import {
    Modal,
    DataTable,
    Table,
    TableHead,
    TableRow,
    TableHeader,
    TableBody,
    TableCell,
    TableSelectAll,
    TableSelectRow,
    TableContainer,
    TableToolbar,
    TableToolbarContent,
    TableToolbarSearch,
    Loading,
    MultiSelect,
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { userStore } from '../../admin/users/stores/userStore';
import { User } from '../../admin/users/types/user.types';

interface SelectUsersModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSelect: (users: User[]) => void;
    excludeUserIds?: string[];
    multiSelect?: boolean;
    maxSelect?: number;
    title?: string;
}

export const SelectUsersModal: React.FC<SelectUsersModalProps> = observer(({
                                                                               isOpen,
                                                                               onClose,
                                                                               onSelect,
                                                                               excludeUserIds = [],
                                                                               multiSelect = false,
                                                                               maxSelect,
                                                                               title = 'Выбор пользователей'
                                                                           }) => {
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedDepartments, setSelectedDepartments] = useState<string[]>([]);
    const [selectedPositions, setSelectedPositions] = useState<string[]>([]);
    const [selectedRows, setSelectedRows] = useState<User[]>([]);

    const [allDepartments, setAllDepartments] = useState<Array<{ id: string; name: string }>>([]);
    const [allPositions, setAllPositions] = useState<Array<{ id: string; name: string }>>([]);

    useEffect(() => {
        if (isOpen) {
            loadData();
        }
    }, [isOpen]);

    useEffect(() => {
        if (isOpen && userStore.users.length > 0) {
            // Собираем уникальные департаменты и должности
            const departmentsMap = new Map<string, string>();
            const positionsMap = new Map<string, string>();

            userStore.users.forEach(user => {
                if (user.departmentId && user.department) {
                    departmentsMap.set(user.departmentId, user.department);
                }
                if (user.positionId && user.position) {
                    positionsMap.set(user.positionId, user.position);
                }
            });

            setAllDepartments(Array.from(departmentsMap.entries()).map(([id, name]) => ({ id, name })));
            setAllPositions(Array.from(positionsMap.entries()).map(([id, name]) => ({ id, name })));
        }
    }, [userStore.users, isOpen]);

    const loadData = async () => {
            await userStore.loadUsers();
    };
    
    const filteredUsers = useMemo(() => {
        let users = userStore.users.filter(u => !excludeUserIds.includes(u.id));
        
        if (searchTerm) {
            const term = searchTerm.toLowerCase();
            users = users.filter(u =>
                `${u.lastName} ${u.firstName} ${u.secondName || ''}`.toLowerCase().includes(term) ||
                u.email.toLowerCase().includes(term)
            );
        }
        
        if (selectedDepartments.length > 0) {
            users = users.filter(u => selectedDepartments.includes(u.departmentId));
        }
        
        if (selectedPositions.length > 0) {
            users = users.filter(u => selectedPositions.includes(u.positionId));
        }

        return users;
    }, [userStore.users, searchTerm, selectedDepartments, selectedPositions, excludeUserIds]);

    const handleSelectRow = (user: User, isSelected: boolean) => {
        if (multiSelect) {
            if (isSelected) {
                if (maxSelect && selectedRows.length >= maxSelect) {
                    return;
                }
                setSelectedRows([...selectedRows, user]);
            } else {
                setSelectedRows(selectedRows.filter(u => u.id !== user.id));
            }
        } else {
            setSelectedRows(isSelected ? [user] : []);
        }
    };

    const handleSelectAll = (isSelected: boolean) => {
        if (isSelected) {
            const selectableUsers = filteredUsers.slice(0, maxSelect || filteredUsers.length);
            setSelectedRows(selectableUsers);
        } else {
            setSelectedRows([]);
        }
    };

    const handleSubmit = () => {
        onSelect(selectedRows);
        handleClose();
    };

    const handleClose = () => {
        setSearchTerm('');
        setSelectedDepartments([]);
        setSelectedPositions([]);
        setSelectedRows([]);
        onClose();
    };

    const isRowSelected = (userId: string) => {
        return selectedRows.some(u => u.id === userId);
    };

    const headers = [
        { key: 'fullName', header: 'ФИО' },
        { key: 'email', header: 'Email' },
        { key: 'position', header: 'Должность' },
        { key: 'department', header: 'Подразделение' },
        { key: 'boss', header: 'Руководитель' },
    ];

    const rows = filteredUsers.map(user => ({
        id: user.id.toString(),
        fullName: `${user.lastName} ${user.firstName} ${user.secondName || ''}`,
        email: user.email,
        position: user.position,
        department: user.department,
        boss: user.bossName || '—',
        user: user
    }));

    if (userStore.isLoading) {
        return (
            <Modal open={isOpen} onRequestClose={handleClose} modalHeading={title} size="lg">
                <Loading description="Загрузка пользователей..." />
            </Modal>
        );
    }

    return (
        <Modal
            open={isOpen}
            onRequestClose={handleClose}
            modalHeading={title}
            size="md"
            primaryButtonText="Выбрать"
            secondaryButtonText="Отмена"
            onRequestSubmit={handleSubmit}
            primaryButtonDisabled={multiSelect && maxSelect ? selectedRows.length < (maxSelect === 10 ? 2 : 1) : false}
        >
            <div style={{ marginBottom: '1rem', display: 'flex', gap: '1rem' }}>
                <div style={{ flex: 1 }}>
                    <MultiSelect
                        id="departments-filter"
                        titleText="Подразделения"
                        items={allDepartments.map(d => ({ id: d.id, label: d.name }))}
                        itemToString={(item) => item?.label || ''}
                        selectionFeedback="top-after-reopen"
                        onChange={({ selectedItems }) => {
                            selectedItems = selectedItems ?? [];
                            setSelectedDepartments(selectedItems.map(item => item.id));
                        }}
                        label="Выберите подразделения"
                    />
                </div>
                <div style={{ flex: 1 }}>
                    <MultiSelect
                        id="positions-filter"
                        titleText="Должности"
                        items={allPositions.map(p => ({ id: p.id, label: p.name }))}
                        itemToString={(item) => item?.label || ''}
                        selectionFeedback="top-after-reopen"
                        onChange={({ selectedItems }) => {
                            selectedItems = selectedItems ?? [];
                            setSelectedPositions(selectedItems.map(item => item.id));
                        }}
                        label="Выберите должности"
                    />
                </div>
            </div>

            <DataTable rows={rows} headers={headers}>
                {({
                      rows,
                      headers,
                      getTableProps,
                      getHeaderProps,
                      getRowProps,
                      getSelectionProps,
                      getTableContainerProps,
                      onInputChange,
                  }) => (
                    <TableContainer {...getTableContainerProps()}>
                        <TableToolbar>
                            <TableToolbarContent>
                                <TableToolbarSearch
                                    onChange={(e) => {
                                        onInputChange(e);
                                        setSearchTerm(e.toString());
                                    }}
                                />
                            </TableToolbarContent>
                        </TableToolbar>

                        <Table {...getTableProps()}>
                            <TableHead>
                                <TableRow>
                                    <TableSelectAll
                                        {...getSelectionProps()}
                                        checked={selectedRows.length === filteredUsers.length && filteredUsers.length > 0}
                                        indeterminate={selectedRows.length > 0 && selectedRows.length < filteredUsers.length}
                                        onSelect={() => handleSelectAll(selectedRows.length === 0)}
                                    />
                                    {headers.map(header => (
                                        <TableHeader {...getHeaderProps({ header })} key={header.key}>
                                            {header.header}
                                        </TableHeader>
                                    ))}
                                </TableRow>
                            </TableHead>

                            <TableBody>
                                {rows.map(row => {
                                    const user = row.cells.find(c => c.info.header === 'fullName')?.value;
                                    const originalUser = filteredUsers.find(u =>
                                        `${u.lastName} ${u.firstName} ${u.secondName || ''}` === user
                                    );

                                    return (
                                        <TableRow
                                            {...getRowProps({ row })}
                                            key={row.id}
                                            onClick={() => {
                                                if (multiSelect) {
                                                    handleSelectRow(originalUser!, !isRowSelected(row.id));
                                                } else {
                                                    handleSelectRow(originalUser!, true);
                                                    setTimeout(handleSubmit, 100);
                                                }
                                            }}
                                            style={{ cursor: 'pointer' }}
                                        >
                                            <TableSelectRow
                                                {...getSelectionProps({ row })}
                                                checked={isRowSelected(row.id)}
                                                onSelect={(e) => {
                                                    e.stopPropagation();
                                                    handleSelectRow(originalUser!, !isRowSelected(row.id));
                                                }}
                                            />
                                            {row.cells.map(cell => (
                                                <TableCell key={cell.id}>
                                                    {cell.value}
                                                </TableCell>
                                            ))}
                                        </TableRow>
                                    );
                                })}
                            </TableBody>
                        </Table>
                    </TableContainer>
                )}
            </DataTable>
        </Modal>
    );
});