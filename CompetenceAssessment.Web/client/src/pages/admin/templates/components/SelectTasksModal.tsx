// src/components/TemplateModal/SelectTasksModal.tsx
import React, { useState, useEffect, useMemo } from 'react';
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

    useEffect(() => {
        if (isOpen) {
            taskStore.loadTasks();
            setSelectedIds([]);
            setSearchTerm('');
        }
    }, [isOpen]);
    
    const availableTasks = useMemo(() => {
        let filtered = taskStore.tasks.filter(task => !existingTaskIds.includes(task.id));

        if (templateType === 1) {
            filtered = filtered.filter(task => task.type == '1' || task.type == '2');
        } else if (templateType === 2) {
            filtered = filtered.filter(task => task.type == '3');
        }

        return filtered;
    }, [taskStore.tasks, existingTaskIds, templateType]);

    const filteredTasks = useMemo(() => {
        if (!searchTerm) return availableTasks;
        const term = searchTerm.toLowerCase();
        return availableTasks.filter(
            task => task.text.toLowerCase().includes(term)
        );
    }, [availableTasks, searchTerm]);

    const handleSelectAll = () => {
        if (selectedIds.length === filteredTasks.length) {
            setSelectedIds([]);
        } else {
            setSelectedIds(filteredTasks.map(c => c.id));
        }
    };

    const handleSelectRow = (id: string) => {
        if (selectedIds.includes(id)) {
            setSelectedIds(selectedIds.filter(i => i !== id));
        } else {
            setSelectedIds([...selectedIds, id]);
        }
    };

    const handleAdd = () => {
        const selectedTasks = taskStore.tasks.filter(
            c => selectedIds.includes(c.id)
        );
        onAdd(selectedTasks);
        onClose();
    };

    const getTaskTypeLabel = (type: string): string => {
        switch (Number(type)) {
            case 1: return 'Тест';
            case 2: return 'Открытый';
            case 3: return 'Анкета';
            default: return 'Неизвестно';
        }
    };

    const headers = [
        { key: 'text', header: 'Текст задания' },
        { key: 'type', header: 'Тип' }
    ];

    const rows = filteredTasks.map(task => ({
        id: task.id,
        text: task.text,
        type: getTaskTypeLabel(task.type)
    }));

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
                    onChange={(e) => setSearchTerm(e.toString)}
                />
            </div>

            <TableContainer>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableSelectAll
                                id="id"
                                name="name"
                                checked={selectedIds.length === filteredTasks.length && filteredTasks.length > 0}
                                indeterminate={selectedIds.length > 0 && selectedIds.length < filteredTasks.length}
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
                            <TableRow key={row.id.toString()}>
                                <TableSelectRow
                                    id={row.id.toString()}
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

            {filteredTasks.length === 0 && (
                <div style={{ textAlign: 'center', padding: '2rem', color: '#6f6f6f' }}>
                    {searchTerm ? 'Ничего не найдено' : 'Нет доступных заданий для выбранного типа шаблона'}
                </div>
            )}
        </Modal>
    );
});