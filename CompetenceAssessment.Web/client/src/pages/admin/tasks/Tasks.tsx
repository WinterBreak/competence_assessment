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
import { Edit, TrashCan, Add } from '@carbon/react/icons';
import { taskStore } from './stores/taskStore';
import {TaskModal} from "../tasks/components/TaskModal";
import {Task} from "./types/task.types";
import {CreateTaskDto, UpdateTaskDto} from "../tasks/types/task.types";

export const Tasks: React.FC = observer(() => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingTask, setEditingTask] = useState<Task | null>(null);
    
    useEffect(() => {
        taskStore.loadTasks();
    }, []);

    const handleEdit = (id: string) => {
        let task = taskStore.tasks.find(c => c.id.toString() === id) ?? null;
        setEditingTask(task);
        setIsModalOpen(true);
    };

    const handleDelete = async (id: string) => {
        await taskStore.deleteTask(id);
    };

    const handleAdd = () => {
        setEditingTask(null);
        setIsModalOpen(true);
    };

    const handleModalSubmit = async (data: { id?: string; text: string; type: string; answers?: Record<string, boolean> }) => {
        if (editingTask) {
            let editDto: UpdateTaskDto = {
                id: Number(data.id),
                text: data.text,
                type: Number(data.type),
                answers: data.answers || {}
            };
            await taskStore.updateTask(editDto);

            // Обновляем editingTask после успешного сохранения
            const updatedTask = taskStore.tasks.find(t => t.id.toString() === data.id);
            if (updatedTask) {
                setEditingTask(updatedTask);
            }
            // НЕ закрываем модальное окно при редактировании
        } else {
            let createDto: CreateTaskDto = {
                text: data.text,
                type: Number(data.type),
                answers: data.answers || {}
            };
            await taskStore.createTask(createDto);
            setIsModalOpen(false); // Закрываем только при создании
            setEditingTask(null);
        }
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
        setEditingTask(null);
    };

    const handleExport = async () => {
        await taskStore.exportTasks();
    };

    const taskType = (type: string | number) => {
        switch (Number(type)) {
            case 1: return 'Тестовый вопрос';
            case 2: return 'Открытый вопрос';
            case 3: return 'Анкета';
            default: return 'Неизвестный тип';
        }
    }
    
    const rows = useMemo(() =>
            taskStore.tasks.map(t => ({
                id: String(t.id),
                text: t.text,
                type: taskType(t.type),
            })),
        [taskStore.tasks]);

    if (taskStore.isLoading && taskStore.tasks.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'text', header: 'Текст задания' },
        { key: 'type', header: 'Тип' },
        { key: 'actions', header: 'Действия' }
    ];

    return (
        <div>
            {taskStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={taskStore.error}
                    onClose={() => taskStore.clearError()}
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
                                            taskStore.setSearchTerm(e.toString());
                                        }}
                                    />

                                    <TableToolbarMenu>
                                        <TableToolbarAction onClick={handleExport}>
                                            Экспорт
                                        </TableToolbarAction>
                                    </TableToolbarMenu>

                                    {selectedIds.length > 0 && (
                                        <Button
                                            kind="danger"
                                            renderIcon={TrashCan}
                                            onClick={() =>
                                                taskStore.deleteManyTasks(selectedIds)
                                            }
                                        >
                                            Удалить ({selectedIds.length})
                                        </Button>
                                    )}

                                    <Button
                                        kind="primary"
                                        onClick={handleAdd}
                                        renderIcon={Add}
                                    >
                                        Добавить
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
                                                            <Button
                                                                kind="ghost"
                                                                size="sm"
                                                                iconDescription="Редактировать"
                                                                onClick={() => handleEdit(row.id)}
                                                                renderIcon={Edit}
                                                                hasIconOnly
                                                            />
                                                            <Button
                                                                kind="ghost"
                                                                size="sm"
                                                                iconDescription="Удалить"
                                                                onClick={() => handleDelete(row.id)}
                                                                renderIcon={TrashCan}
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
                page={taskStore.currentPage}
                pageSize={taskStore.pageSize}
                totalItems={taskStore.totalItems}
                pageSizes={[5, 10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== taskStore.pageSize) {
                        taskStore.setPageSize(pageSize);
                    } else {
                        taskStore.setCurrentPage(page);
                    }
                }}
            />
            
            <TaskModal
                isOpen={isModalOpen}
                onClose={handleModalClose}
                onSubmit={handleModalSubmit}
                initialData={editingTask || undefined}
                mode={editingTask ? 'edit' : 'create'}
            />
        </div>
    );
});