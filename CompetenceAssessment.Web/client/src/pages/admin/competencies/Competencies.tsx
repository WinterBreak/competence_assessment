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
    TableToolbarAction,
    TableToolbarSearch,
    Loading,
    ToastNotification, IconButton, Modal
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { Edit, TrashCan, Add } from '@carbon/react/icons';
import { competenceStore } from './stores/competenceStore';
import {Competence, CreateCompetencyDto, UpdateCompetencyDto} from "./types/competence.types";
import { CompetenceModal } from './components/CompetenceModal';
import {competenceModelStore} from "../competendeModels/stores/competenceModelStore";

export const Competencies: React.FC = observer(() => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
    const [editingCompetence, setEditingCompetence] = useState<Competence | null>(null);
    const [itemToDelete, setItemToDelete] = useState<string | null>(null);
    
    useEffect(() => {
        competenceStore.loadCompetencies();
    }, []);

    const handleEdit = (id: string) => {
        let competence = competenceStore.competencies.find(c => c.id.toString() === id) ?? null;
        setEditingCompetence(competence);
        setIsModalOpen(true);
    };

    const handleDeleteClick = (id: string) => {
        setItemToDelete(id);
        setIsDeleteModalOpen(true);
    };

    const handleAdd = () => {
        setEditingCompetence(null);
        setIsModalOpen(true);
    };

    const handleModalSubmit = async (data: { id?: string; name: string; description?: string }) => {
        if (editingCompetence) {
            let editDto: UpdateCompetencyDto = {
                id: Number(data.id),
                name: data.name,
                description: data.description
            };
            await competenceStore.updateCompetency(editDto);
        } else {
            let createDto: CreateCompetencyDto = {
                name: data.name,
                description: data.description
            };
            await competenceStore.createCompetency(createDto);
        }
        setIsModalOpen(false);
        setEditingCompetence(null);
    };

    const handleModalClose = () => {
        setIsModalOpen(false);
        setEditingCompetence(null);
    };

    const handleExport = async () => {
        await competenceStore.exportCompetencies();
    };

    const confirmDelete = async () => {
        if (itemToDelete) {
            await competenceStore.deleteCompetency(itemToDelete);
            setItemToDelete(null);
        }
        setIsDeleteModalOpen(false);
    };

    const rows = competenceStore.competencies.map(comp => ({
                id: String(comp.id),
                name: comp.name,
                description: comp.description,
            }));

    if (competenceStore.isLoading && competenceStore.competencies.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'name', header: 'Название' },
        { key: 'description', header: 'Описание' },
        { key: 'actions', header: 'Действия' }
    ];

    return (
        <div>
            {competenceStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={competenceStore.error}
                    onClose={() => competenceStore.clearError()}
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
                            title="Компетенции"
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            competenceStore.setSearchTerm(e.toString());
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
                                                competenceStore.deleteManyCompetencies(selectedIds)
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
                                                            <IconButton
                                                                kind="ghost"
                                                                size="sm"
                                                                label="Редактировать"
                                                                onClick={() => handleEdit(row.id) }
                                                            ><Edit/></IconButton>
                                                            <Button
                                                                kind="ghost"
                                                                size="sm"
                                                                iconDescription="Удалить"
                                                                onClick={() => handleDeleteClick(row.id)}
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
                page={competenceModelStore.currentPage}
                pageSize={competenceModelStore.pageSize}
                totalItems={competenceModelStore.totalItems}
                backwardText="Назад"
                forwardText="Вперед"
                itemRangeText={(min, max, total) => `${ min }–${ max } из ${ total } элементов`}
                itemsPerPageText="Элементов на странице"
                pageRangeText={(_current, total) => `из ${ total } ${ total === 1 ? 'страницы' : 'страниц' }`}
                pageSizes={[10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== competenceModelStore.pageSize) {
                        competenceModelStore.setPageSize(pageSize);
                    } else {
                        competenceModelStore.setCurrentPage(page);
                    }
                }}
            />

            <CompetenceModal
                isOpen={isModalOpen}
                onClose={handleModalClose}
                onSubmit={handleModalSubmit}
                initialData={editingCompetence || undefined}
                mode={editingCompetence ? 'edit' : 'create'}
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
                <p>Вы уверены, что хотите удалить эту компетенцию?</p>
                <p style={{ marginTop: '0.5rem', color: '#da1e28' }}>
                    Это действие невозможно отменить.
                </p>
            </Modal>
        </div>
    );
});