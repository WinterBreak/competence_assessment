import React, { useEffect, useMemo } from 'react';
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
import { competenceModelStore } from './stores/competenceModelStore';

export const CompetenceModels: React.FC = observer(() => {

    useEffect(() => {
        competenceModelStore.loadCompetenceModels();
    }, []);

    const handleEdit = (id: string) => {
        console.log('Edit:', id);
    };

    const handleDelete = async (id: string) => {
        await competenceModelStore.deleteCompetenceModel(id);
    };

    const handleAdd = () => {
        console.log('Add');
    };

    const handleExport = async () => {
        await competenceModelStore.exportCompetenceModels();
    };

    const rows = useMemo(() =>
            competenceModelStore.competenceModels.map(cm => ({
                id: String(cm.id),
                name: cm.name,
                description: cm.description,
                creation_date: cm.creationDate.toString(),
            })),
        [competenceModelStore.competenceModels]);

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
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            competenceModelStore.setSearchTerm(e.toString());
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
                page={competenceModelStore.currentPage}
                pageSize={competenceModelStore.pageSize}
                totalItems={competenceModelStore.totalItems}
                pageSizes={[5, 10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== competenceModelStore.pageSize) {
                        competenceModelStore.setPageSize(pageSize);
                    } else {
                        competenceModelStore.setCurrentPage(page);
                    }
                }}
            />
        </div>
    );
});