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
import { templateStore } from './stores/templateStore';

export const Templates: React.FC = observer(() => {

    useEffect(() => {
        templateStore.loadTemplates();
    }, []);

    const handleEdit = (id: string) => {
        console.log('Edit:', id);
    };

    const handleDelete = async (id: string) => {
        await templateStore.deleteTemplate(id);
    };

    const handleAdd = () => {
        console.log('Add');
    };

    const handleExport = async () => {
        await templateStore.exportTemplates();
    };

    const rows = useMemo(() =>
            templateStore.templates.map(t => ({
                id: String(t.id),
                name: t.name,
                type: t.type,
                
                creation_date: t.creationDate.toString(),
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
                                            templateStore.setSearchTerm(e.toString());
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
        </div>
    );
});