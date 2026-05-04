import React, {useEffect, useMemo, useState} from 'react';
import { useNavigate } from 'react-router-dom';
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
import { assessmentStore } from './stores/assessmentStore';
import { Assessment } from "./types/assessment.types";
import { View, Result, Document } from '@carbon/react/icons';

export const AssessmentHistory: React.FC = observer(() => {
    const navigate = useNavigate();

    useEffect(() => {
        assessmentStore.loadHistory();
    }, []);

    const assessmentType = (type: string) => {
        switch (Number(type)){
            case 1: return 'Тестирование';
            case 2: return 'Анкетирование';
            case 3: return 'Оценка 360 градусов';
            default: return 'Неизвестно';
        }
    }

    const rows = useMemo(() =>
            assessmentStore.assessments.map(a => ({
                id: String(a.id),
                type: assessmentType(a.type),
                startDate: a.startDate ? new Date(a.startDate).toLocaleString('ru-RU') : '',
                endDate: a.endDate ? new Date(a.endDate).toLocaleString('ru-RU') : '',
                isFinished: a.isFinished,
                candidateId: a.candidate.id,
            })),
        [assessmentStore.assessments]);

    if (assessmentStore.isLoading && assessmentStore.assessments.length === 0) {
        return <Loading description="Загрузка..." withOverlay />;
    }

    const headers = [
        { key: 'type', header: 'Метод оценки' },
        { key: 'startDate', header: 'Дата начала' },
        { key: 'endDate', header: 'Дата завершения' },
        { key: 'isFinished', header: 'Статус' },
        { key: 'actions', header: 'Действия' },
    ];

    return (
        <div>
            {assessmentStore.error && (
                <ToastNotification
                    kind="error"
                    title="Ошибка"
                    subtitle={assessmentStore.error}
                    onClose={() => assessmentStore.clearError()}
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
                            title="История оценок"
                            description="Список всех проведенных и запланированных оценок"
                            {...getTableContainerProps()}
                        >
                            <TableToolbar {...getToolbarProps()}>
                                <TableToolbarContent>
                                    <TableToolbarSearch
                                        onChange={(e) => {
                                            onInputChange(e);
                                            assessmentStore.setSearchTerm(e.toString());
                                        }}
                                        placeholder="Поиск по истории..."
                                    />

                                    <TableToolbarMenu>
                                        <TableToolbarAction onClick={() => console.log('Экспорт')}>
                                            Экспорт в Excel
                                        </TableToolbarAction>
                                    </TableToolbarMenu>
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
                                    {rows.map(row => {
                                        const originalAssessment = assessmentStore.assessments.find(a => String(a.id) === row.id);
                                        const isFinished = originalAssessment?.isFinished;

                                        return (
                                            <TableRow {...getRowProps({ row })} key={row.id}>
                                                <TableSelectRow {...getSelectionProps({ row })} />

                                                {row.cells.map(cell => {
                                                    if (cell.info.header === 'isFinished') {
                                                        return (
                                                            <TableCell key={cell.id}>
                                                                    <span style={{
                                                                        display: 'inline-flex',
                                                                        alignItems: 'center',
                                                                        gap: '0.25rem',
                                                                        color: '#198038'
                                                                    }}>
                                                                        Завершено
                                                                    </span>
                                                            </TableCell>
                                                        );
                                                    }
                                                    
                                                    if (cell.info.header === 'actions') {
                                                        return (
                                                            <TableCell key={cell.id}>
                                                                <div style={{ display: 'flex', gap: '0.5rem' }}>
                                                                    <Button
                                                                                kind="ghost"
                                                                                size="sm"
                                                                                onClick={() => navigate(`/results/${row.id}`)}
                                                                                renderIcon={Result}
                                                                                iconDescription="Просмотреть результаты"
                                                                                tooltipAlignment="center"
                                                                            >
                                                                                Результаты
                                                                            </Button>
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
                                        );
                                    })}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    );
                }}
            </DataTable>

            <Pagination
                page={assessmentStore.currentPage}
                pageSize={assessmentStore.pageSize}
                totalItems={assessmentStore.totalItems}
                backwardText="Назад"
                forwardText="Вперед"
                itemRangeText={(min, max, total) => `${min}–${max} из ${total} элементов`}
                itemsPerPageText="Элементов на странице"
                pageRangeText={(_current, total) => `из ${total} ${total === 1 ? 'страницы' : 'страниц'}`}
                pageSizes={[10, 20, 50]}
                onChange={({ page, pageSize }) => {
                    if (pageSize !== assessmentStore.pageSize) {
                        assessmentStore.setPageSize(pageSize);
                    } else {
                        assessmentStore.setCurrentPage(page);
                    }
                }}
            />
        </div>
    );
});