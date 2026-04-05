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
    ToastNotification, IconButton
} from '@carbon/react';
import { observer } from 'mobx-react-lite';
import { Edit, TrashCan, Add } from '@carbon/react/icons';
import { competenceStore } from './stores/competenceStore';
import {Competence, CreateCompetencyDto, UpdateCompetencyDto} from "./types/competence.types";
import { CompetenceModal } from './components/CompetenceModal';

export const Competencies: React.FC = observer(() => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingCompetence, setEditingCompetence] = useState<Competence | null>(null);
    
    useEffect(() => {
        competenceStore.loadCompetencies();
    }, []);

    const handleEdit = (id: string) => {
        let competence = competenceStore.competencies.find(c => c.id.toString() === id) ?? null;
        setEditingCompetence(competence);
        setIsModalOpen(true);
    };

    const handleDelete = async (id: string) => {
        await competenceStore.deleteCompetency(id);
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
                page={competenceStore.currentPage}
                pageSize={competenceStore.pageSize}
                totalItems={competenceStore.totalItems}
                pageSizes={[5, 10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== competenceStore.pageSize) {
                        competenceStore.setPageSize(pageSize);
                    } else {
                        competenceStore.setCurrentPage(page);
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
        </div>
    );
});