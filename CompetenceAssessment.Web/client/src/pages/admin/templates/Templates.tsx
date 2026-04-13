import React, { useEffect, useMemo, useState } from 'react';
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
    ToastNotification,
    Modal
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { Edit, TrashCan, Add } from '@carbon/react/icons';
import { templateStore } from './stores/templateStore';
import { TemplateModal } from "./components/TemplateModal";
import { Template } from "./types/template.types";

export const Templates: React.FC = observer(() => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingTemplate, setEditingTemplate] = useState<Template | null>(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
    const [itemToDelete, setItemToDelete] = useState<string | null>(null);

    useEffect(() => {
        templateStore.loadTemplates();
    }, []);

    const handleEdit = (id: string) => {
        const template = templateStore.templates.find(t => t.id.toString() === id);
        if (template) {
            setEditingTemplate(template);
            setIsModalOpen(true);
        }
    };

    const handleDeleteClick = (id: string) => {
        setItemToDelete(id);
        setIsDeleteModalOpen(true);
    };

    const confirmDelete = async () => {
        if (itemToDelete) {
            await templateStore.deleteTemplate(itemToDelete);
            setItemToDelete(null);
        }
        setIsDeleteModalOpen(false);
    };

    const handleAdd = () => {
        setEditingTemplate(null);
        setIsModalOpen(true);
    };

    const handleSubmit = async (data: {
        id?: string;
        name: string;
        description?: string;
        type: string;
        scale: number;
        competenceModelId?: string;
        tasks: Array<{ taskId: string; weight: number; competenceId: string }>;
    }) => {
        const weights: Record<number, number> = {};
        data.tasks.forEach(task => {
            weights[Number(task.taskId)] = task.weight;
        });
        
        const competenciesToTasks: Record<number, number[]> = {};
        data.tasks.forEach(task => {
            const competenceId = Number(task.competenceId);
            const taskId = Number(task.taskId);

            if (!competenciesToTasks[competenceId]) {
                competenciesToTasks[competenceId] = [];
            }
            competenciesToTasks[competenceId].push(taskId);
        });

        if (editingTemplate) {
            await templateStore.updateTemplate({
                id: editingTemplate.id,
                name: data.name,
                type: data.type,
                scale: data.scale,
                competenceModelId: data.competenceModelId || '',
                weights: weights,
                competenciesToTasks: competenciesToTasks
            });
        } else {
            await templateStore.createTemplate({
                name: data.name,
                type: data.type,
                scale: data.scale,
                competenceModelId: data.competenceModelId || '',
                weights: weights,
                competenciesToTasks: competenciesToTasks
            });
        }
        setIsModalOpen(false);
        setEditingTemplate(null);
    };

    const handleExport = async () => {
        await templateStore.exportTemplates();
    };

    const rows = useMemo(() =>
            templateStore.templates.map(t => ({
                id: String(t.id),
                name: t.name,
                type: t.type == '1' ? 'Тестирование' : 'Анкета',
                creation_date: t.creationDate ? new Date(t.creationDate).toLocaleDateString('ru-RU') : '',
            })),
        [templateStore.templates]);

    if (templateStore.isLoading && templateStore.templates.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'name', header: 'Название' },
        { key: 'type', header: 'Тип' },
        { key: 'creation_date', header: 'Дата создания' },
        { key: 'actions', header: 'Действия' }
    ];

    return (
        <div>
            {templateStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={templateStore.error}
                    onClose={() => templateStore.clearError()}
                    style={{ marginBottom: '1rem' }}
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
                            title="Шаблоны"
                            description="Управление шаблонами"
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            templateStore.setSearchTerm(e.toString());
                                        }}
                                        placeholder="Поиск шаблонов..."
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
                                                templateStore.deleteManyTemplates(selectedIds)
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
                                                            <div style={{ display: 'flex', gap: '0.5rem' }}>
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
                                                                    onClick={() => handleDeleteClick(row.id)}
                                                                    renderIcon={TrashCan}
                                                                    hasIconOnly
                                                                />
                                                            </div>
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
                page={templateStore.currentPage}
                pageSize={templateStore.pageSize}
                totalItems={templateStore.totalItems}
                pageSizes={[5, 10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== templateStore.pageSize) {
                        templateStore.setPageSize(pageSize);
                    } else {
                        templateStore.setCurrentPage(page);
                    }
                }}
            />

            <TemplateModal
                isOpen={isModalOpen}
                onClose={() => {
                    setIsModalOpen(false);
                    setEditingTemplate(null);
                }}
                onSubmit={handleSubmit}
                initialData={editingTemplate ? {
                    id: editingTemplate.id,
                    name: editingTemplate.name,
                    type: editingTemplate.type,
                    scale: editingTemplate.scale,
                    competenceModel: editingTemplate.competenceModel,
                    tasks: editingTemplate.tasks
                } : undefined}
                mode={editingTemplate ? 'edit' : 'create'}
            />

            <Modal
                open={isDeleteModalOpen}
                modalHeading="Подтверждение удаления"
                primaryButtonText="Удалить"
                secondaryButtonText="Отмена"
                danger
                onRequestClose={() => setIsDeleteModalOpen(false)}
                onRequestSubmit={confirmDelete}
            >
                <p>Вы уверены, что хотите удалить этот шаблон?</p>
                <p style={{ marginTop: '0.5rem', color: '#da1e28' }}>
                    Это действие невозможно отменить.
                </p>
            </Modal>
        </div>
    );
});