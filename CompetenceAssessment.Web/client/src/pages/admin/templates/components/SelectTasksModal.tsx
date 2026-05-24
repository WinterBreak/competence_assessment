// src/components/TemplateModal/SelectTasksModal.tsx
import React, { useState, useEffect, useCallback, useTransition } from 'react';
import {
    Modal,
    Table,
    TableHead,
    TableRow,
    TableHeader,
    TableBody,
    TableCell,
    TableSelectAll,
    TableSelectRow,
    TableContainer,
    TableToolbarSearch,
    Pagination,
    Loading,
} from '@carbon/react';
import { taskStore } from '../../tasks/stores/taskStore';
import { observer } from 'mobx-react-lite';
import { Task } from "../../tasks/types/task.types";

interface SelectTasksModalProps {
    isOpen: boolean;
    onClose: () => void;
    onAdd: (tasks: Task[]) => void;
    existingTaskIds: string[];
    templateType: number;
    competenceId: string;
}

export const SelectTasksModal: React.FC<SelectTasksModalProps> = observer(({
                                                                               isOpen,
                                                                               onClose,
                                                                               onAdd,
                                                                               existingTaskIds,
                                                                               templateType,
                                                                               competenceId
                                                                           }) => {
    const [selectedIds, setSelectedIds] = useState<string[]>([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [isPending, startTransition] = useTransition();

    // Загрузка заданий
    const loadTasks = useCallback(async () => {
        taskStore.currentPage = currentPage;
        taskStore.pageSize = 50;

        await taskStore.loadTasks();

        const filteredTasks = taskStore.tasks.filter(t =>
            !selectedIds!.includes(t.id.toString())
        );
        taskStore.setFilteredTasks(filteredTasks);
    }, [currentPage, pageSize]);

    // Используем startTransition для предотвращения мигания
    const handlePageChange = useCallback(({ page, pageSize: newPageSize }: { page: number; pageSize: number }) => {
        startTransition(() => {
            setCurrentPage(page);
            setPageSize(newPageSize);
        });
    }, []);

    const handleSearchChange = useCallback((event: { target: { value: string } } | string) => {
        const value = typeof event === 'string' ? event : event.target.value;
        startTransition(() => {
            setSearchTerm(value);
            setCurrentPage(1);
        });
    }, []);

    // Загружаем при изменении параметров
    useEffect(() => {
        if (isOpen) {
            loadTasks();
        }
    }, [currentPage, pageSize, searchTerm, loadTasks, isOpen]);

    // Сброс состояния при открытии модалки
    useEffect(() => {
        if (isOpen) {
            startTransition(() => {
                setSelectedIds([]);
                setSearchTerm('');
                setCurrentPage(1);
                setPageSize(10);
            });
        }
    }, [isOpen]);

    const handleSelectAll = useCallback(() => {
        const currentTasks = taskStore.tasks;
        if (selectedIds.length === currentTasks.length && currentTasks.length > 0) {
            setSelectedIds([]);
        } else {
            setSelectedIds(currentTasks.map(c => c.id));
        }
    }, [selectedIds, taskStore.tasks]);

    const handleSelectRow = useCallback((id: string) => {
        setSelectedIds(prev =>
            prev.includes(id) ? prev.filter(i => i !== id) : [...prev, id]
        );
    }, []);

    const handleAdd = useCallback(() => {
        const selectedTasks = taskStore.tasks.filter(
            c => selectedIds.includes(c.id)
        );
        onAdd(selectedTasks);
        onClose();
    }, [selectedIds, taskStore.tasks, onAdd, onClose]);

    const getTaskTypeLabel = useCallback((type: string): string => {
        switch (Number(type)) {
            case 1: return 'Тест';
            case 2: return 'Открытый';
            case 3: return 'Анкета';
            default: return 'Неизвестно';
        }
    }, []);

    const headers = [
        { key: 'text', header: 'Текст задания' },
        { key: 'type', header: 'Тип' }
    ];

    const rows = taskStore.tasks.map(task => ({
        id: task.id,
        text: task.text,
        type: getTaskTypeLabel(task.type)
    }));

    // Показываем старые данные во время загрузки вместо пустой таблицы
    const showLoading = taskStore.isLoading && !isPending;
    const hasData = rows.length > 0;

    return (
        <Modal
            open={isOpen}
            modalHeading="Выбор заданий"
            primaryButtonText="Добавить"
            secondaryButtonText="Отмена"
            onRequestClose={onClose}
            onRequestSubmit={handleAdd}
            size="lg"
            primaryButtonDisabled={selectedIds.length === 0}
        >
            <div style={{ marginBottom: '1rem' }}>
                <TableToolbarSearch
                    persistent
                    placeholder="Поиск заданий..."
                    onChange={handleSearchChange}
                />
            </div>

            <div style={{ position: 'relative', minHeight: '400px' }}>
                {showLoading && (
                    <div style={{
                        position: 'absolute',
                        top: 0,
                        left: 0,
                        right: 0,
                        bottom: 0,
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                        backgroundColor: 'rgba(255, 255, 255, 0.8)',
                        zIndex: 1
                    }}>
                        <Loading description="Загрузка заданий..." />
                    </div>
                )}

                <div style={{ opacity: showLoading ? 0.5 : 1, transition: 'opacity 0.2s' }}>
                    <TableContainer>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableSelectAll
                                        id="select-all-tasks"
                                        name="select-all-tasks"
                                        checked={hasData && selectedIds.length === taskStore.tasks.length}
                                        indeterminate={selectedIds.length > 0 && selectedIds.length < taskStore.tasks.length}
                                        onSelect={handleSelectAll}
                                    />
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
                                        <TableSelectRow
                                            id={row.id}
                                            name={row.text}
                                            checked={selectedIds.includes(row.id)}
                                            onSelect={() => handleSelectRow(row.id)}
                                        />
                                        <TableCell>{row.text}</TableCell>
                                        <TableCell>{row.type}</TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>

                    {!showLoading && !hasData && (
                        <div style={{ textAlign: 'center', padding: '2rem', color: '#6f6f6f' }}>
                            {searchTerm ? 'Ничего не найдено' : 'Нет доступных заданий для выбранного типа шаблона'}
                        </div>
                    )}
                </div>
            </div>

            {taskStore.totalPages > 0 && (
                <Pagination
                    backwardText="Назад"
                    forwardText="Вперед"
                    itemsPerPageText="Записей на странице:"
                    page={currentPage}
                    pageNumberText="Номер страницы"
                    pageSize={pageSize}
                    pageSizes={[5, 10, 20, 30, 50]}
                    totalItems={taskStore.totalItems}
                    onChange={handlePageChange}
                    disabled={showLoading}
                />
            )}
        </Modal>
    );
});