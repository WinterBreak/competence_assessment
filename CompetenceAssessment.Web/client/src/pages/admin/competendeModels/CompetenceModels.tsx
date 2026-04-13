// src/pages/CompetenceModels/CompetenceModels.tsx
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
    Modal,
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { Edit, TrashCan, Add } from '@carbon/react/icons';
import { competenceModelStore } from './stores/competenceModelStore';
import { CompetenceModelModal } from "./components/CompetenceModelModal";
import {CompetenceModel } from "./types/model.types";

export const CompetenceModels: React.FC = observer(() => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingCompetenceModel, setEditingCompetenceModel] = useState<CompetenceModel | null>(null);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
    const [itemToDelete, setItemToDelete] = useState<string | null>(null);

    useEffect(() => {
        competenceModelStore.loadCompetenceModels();
    }, []);

    const handleEdit = (id: string) => {
        const model = competenceModelStore.competenceModels.find(m => m.id.toString() === id);
        if (model) {
            setEditingCompetenceModel(model);
            setIsModalOpen(true);
        }
    };

    const handleDeleteClick = (id: string) => {
        setItemToDelete(id);
        setIsDeleteModalOpen(true);
    };

    const confirmDelete = async () => {
        if (itemToDelete) {
            await competenceModelStore.deleteCompetenceModel(itemToDelete);
            setItemToDelete(null);
        }
        setIsDeleteModalOpen(false);
    };

    const handleAdd = () => {
        setEditingCompetenceModel(null);
        setIsModalOpen(true);
    };

    const handleSubmit = async (data: {
        id?: string;
        name: string;
        description?: string;
        competencies: Array<{ competenceId: string; weight: number }>;
    }) => {
        const weights: Record<string, number> = {};
        data.competencies.forEach(item => {
            weights[item.competenceId] = item.weight;
        });

        if (editingCompetenceModel) {
            await competenceModelStore.updateCompetenceModel({
                id: Number(editingCompetenceModel.id),
                name: data.name,
                description: data.description,
                weights: weights
            });
        } else {
            await competenceModelStore.createCompetenceModel({
                name: data.name,
                description: data.description,
                weights: weights
            });
        }
        setIsModalOpen(false);
        setEditingCompetenceModel(null);
    };

    const handleExport = async () => {
        await competenceModelStore.exportCompetenceModels();
    };

    const rows = useMemo(() =>
            competenceModelStore.competenceModels.map(cm => ({
                id: String(cm.id),
                name: cm.name,
                description: cm.description || '',
                creation_date: cm.creationDate ? new Date(cm.creationDate).toLocaleDateString('ru-RU') : '',
            })),
        [competenceModelStore.competenceModels]
    );

    if (competenceModelStore.isLoading && competenceModelStore.competenceModels.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'name', header: 'Название' },
        { key: 'description', header: 'Описание' },
        { key: 'creation_date', header: 'Дата создания' },
        { key: 'actions', header: 'Действия' }
    ];

    return (
        <div>
            {competenceModelStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={competenceModelStore.error}
                    onClose={() => competenceModelStore.clearError()}
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
                            title="Модели компетенций"
                            description="Управление моделями компетенций"
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            competenceModelStore.setSearchTerm(e.toString());
                                        }}
                                        placeholder="Поиск моделей..."
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
                                                competenceModelStore.deleteManyCompetenceModels(selectedIds)
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
                page={competenceModelStore.currentPage}
                pageSize={competenceModelStore.pageSize}
                totalItems={competenceModelStore.totalItems}
                backwardText="Назад"
                forwardText="Вперед"
                itemRangeText={(min, max, total) => `${ min }–${ max } из ${ total } элементов`}
                itemsPerPageText="Элементов на странице"
                pageRangeText={(_current, total) => `из ${ total } ${ total === 1 ? 'страницы' : 'страниц' }`}
                pageSizes={[5, 10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== competenceModelStore.pageSize) {
                        competenceModelStore.setPageSize(pageSize);
                    } else {
                        competenceModelStore.setCurrentPage(page);
                    }
                }}
            />
            
            <CompetenceModelModal
                isOpen={isModalOpen}
                onClose={() => {
                    setIsModalOpen(false);
                    setEditingCompetenceModel(null);
                }}
                onSubmit={handleSubmit}
                initialData={editingCompetenceModel || undefined}
                mode={editingCompetenceModel ? 'edit' : 'create'}
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
                <p>Вы уверены, что хотите удалить эту модель компетенций?</p>
                <p style={{ marginTop: '0.5rem', color: '#da1e28' }}>
                    Это действие невозможно отменить.
                </p>
            </Modal>
        </div>
    );
});