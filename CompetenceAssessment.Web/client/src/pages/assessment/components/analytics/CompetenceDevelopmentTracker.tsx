import React, { useState, useEffect } from 'react';
import { LineChart, Line, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { DatePicker, DatePickerInput, Tag, Loading } from '@carbon/react';
import { CompetenceDevelopmentDto, DevelopmentPointDto } from "../../types/assessment.types";
import {assessmentService} from "../../services/assessmentService";

interface CompetenceDevelopmentTrackerProps {
    employeeId: string;
}

export const CompetenceDevelopmentTracker: React.FC<CompetenceDevelopmentTrackerProps> = ({ employeeId }) => {
    const [data, setData] = useState<CompetenceDevelopmentDto>();
    const [loading, setLoading] = useState(true);
    const [dateRange, setDateRange] = useState<[Date, Date] | null>(null);
    const [selectedCompetenceId, setSelectedCompetenceId] = useState<string | null>(null);
    
    const competencies = data?.developmentHistory
        ? Array.from(
            new Map(
                data.developmentHistory.map(point => [point.competenceId, {
                    id: point.competenceId,
                    name: point.competenceName
                }])
            ).values()
        )
        : [];

    const currentCompetence = competencies.find(c => c.id === selectedCompetenceId) || competencies[0];

    const competenceHistory = data?.developmentHistory && currentCompetence
        ? data.developmentHistory
            .filter(point => point.competenceId === currentCompetence.id)
            .sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime())
        : [];
    
    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const response = await assessmentService.getCompetenceDevelopment(employeeId);
                setData(response);
                
                if (response?.developmentHistory?.length > 0 && !selectedCompetenceId) {
                    setSelectedCompetenceId(response.developmentHistory[0].competenceId);
                }
            } catch (error) {
                console.error('Ошибка загрузки данных:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [employeeId]);
    
    useEffect(() => {
        if (competenceHistory.length > 0 && !dateRange) {
            const startDate = new Date(competenceHistory[0].date);
            const endDate = new Date(competenceHistory[competenceHistory.length - 1].date);
            setDateRange([startDate, endDate]);
        }
    }, [competenceHistory]);
    
    if (loading) {
        return (
            <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '400px' }}>
                <Loading description="Загрузка данных развития..." />
            </div>
        );
    }

    if (!data || !data.developmentHistory || data.developmentHistory.length === 0) {
        return (
            <div style={{ padding: '2rem', textAlign: 'center', backgroundColor: 'white', borderRadius: '8px' }}>
                <h3>Нет данных о развитии компетенций</h3>
                <p>Для сотрудника {data?.employeeName || `ID: ${employeeId}`} пока нет записей об оценках.</p>
            </div>
        );
    }

    if (!currentCompetence) {
        return (
            <div style={{ padding: '2rem', textAlign: 'center' }}>
                <p>Нет доступных компетенций для отображения</p>
            </div>
        );
    }
    
    const filteredHistory = competenceHistory.filter(point => {
        if (!dateRange) return true;
        const pointDate = new Date(point.date);
        return pointDate >= dateRange[0] && pointDate <= dateRange[1];
    });

    const chartData = filteredHistory.map(point => ({
        date: new Date(point.date).toLocaleDateString('ru-RU'),
        score: point.score,
        competenceId: point.competenceId,
        fullDate: point.date
    }));

    const getTrend = () => {
        if (filteredHistory.length < 2) return 'Недостаточно данных';
        const first = filteredHistory[0].score;
        const last = filteredHistory[filteredHistory.length - 1].score;
        const difference = last - first;

        if (difference > 5) return '📈 Положительная динамика';
        if (difference < -5) return '📉 Отрицательная динамика';
        return '➡️ Стабильный результат';
    };

    const getProjection = () => {
        if (filteredHistory.length < 2) return null;

        const lastScore = filteredHistory[filteredHistory.length - 1].score;
        const avgImprovement = (filteredHistory[filteredHistory.length - 1].score - filteredHistory[0].score) / filteredHistory.length;

        if (avgImprovement <= 0) return null;

        const assessmentsNeeded = Math.ceil((data.targetScore - lastScore) / avgImprovement);
        return assessmentsNeeded > 0 ? assessmentsNeeded : 0;
    };

    const projection = getProjection();

    return (
        <div style={{ padding: '2rem', backgroundColor: 'white', borderRadius: '8px' }}>
            <div style={{ marginBottom: '2rem' }}>
                <h2>Трекер развития компетенций</h2>
                <p><strong>Сотрудник:</strong> {data.employeeName}</p>
                <p><strong>Целевой уровень:</strong> {data.targetScore} баллов</p>
            </div>

            {/* Селектор компетенций */}
            <div style={{ marginBottom: '2rem' }}>
                <h4>Компетенция:</h4>
                <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
                    {competencies.map(comp => (
                        <Tag
                            key={comp.id}
                            type={selectedCompetenceId === comp.id ? 'blue' : 'gray'}
                            onClick={() => setSelectedCompetenceId(comp.id)}
                            style={{ cursor: 'pointer' }}
                        >
                            {comp.name}
                        </Tag>
                    ))}
                </div>
            </div>

            <div style={{ marginBottom: '2rem' }}>
                <h4>Динамика: {currentCompetence.name}</h4>
                <Tag type={getTrend().includes('📈') ? 'green' : getTrend().includes('📉') ? 'red' : 'gray'}>
                    {getTrend()}
                </Tag>
            </div>

            <div style={{ marginBottom: '2rem' }}>
                <DatePicker
                    datePickerType="range"
                    onChange={(dates) => {
                        if (dates && dates.length === 2) {
                            setDateRange(dates as [Date, Date]);
                        }
                    }}
                    value={dateRange ? [dateRange[0], dateRange[1]] : undefined}
                >
                    <DatePickerInput
                        id="start-date"
                        placeholder="мм/дд/гггг"
                        labelText="Дата начала"
                    />
                    <DatePickerInput
                        id="end-date"
                        placeholder="мм/дд/гггг"
                        labelText="Дата окончания"
                    />
                </DatePicker>
            </div>

            <div style={{ height: '400px', marginBottom: '2rem' }}>
                <ResponsiveContainer>
                    <LineChart data={chartData}>
                        <XAxis
                            dataKey="date"
                            label={{ value: 'Дата оценки', position: 'insideBottom', offset: -5 }}
                        />
                        <YAxis
                            domain={[0, 100]}
                            label={{ value: 'Уровень развития (баллы)', angle: -90, position: 'insideLeft' }}
                        />
                        <Tooltip
                            formatter={(value: any) => [`${value} баллов`, 'Текущий уровень']}
                            labelFormatter={(label) => `Дата: ${label}`}
                        />
                        <Legend />
                        <Line
                            type="monotone"
                            dataKey="score"
                            stroke="#0f62ac"
                            name="Текущий уровень"
                            strokeWidth={2}
                            dot={{ r: 4 }}
                            activeDot={{ r: 6 }}
                        />
                        <Line
                            type="monotone"
                            dataKey={() => data.targetScore}
                            stroke="#da1e28"
                            name="Целевой уровень"
                            strokeDasharray="5 5"
                            strokeWidth={2}
                            dot={false}
                        />
                    </LineChart>
                </ResponsiveContainer>
            </div>

            {projection !== null && projection > 0 && (
                <div style={{
                    padding: '1rem',
                    backgroundColor: '#e8f4ff',
                    borderRadius: '8px',
                    marginTop: '1rem'
                }}>
                    <h4>📊 Прогноз достижения цели</h4>
                    <p>
                        При сохранении текущего темпа развития, целевой уровень ({data.targetScore} баллов)
                        будет достигнут через <strong>{projection}</strong>{' '}
                        {projection === 1 ? 'оценку' : projection >= 2 && projection <= 4 ? 'оценки' : 'оценок'}.
                    </p>
                    {projection > 10 && (
                        <p style={{ color: '#da1e28', marginTop: '0.5rem' }}>
                            ⚠️ Рекомендуется усилить программу развития для данной компетенции.
                        </p>
                    )}
                </div>
            )}

            {filteredHistory.length > 0 && (
                <div style={{ marginTop: '1rem', fontSize: '0.875rem', color: '#525252' }}>
                    <p>Всего оценок в выбранном периоде: {filteredHistory.length}</p>
                    <p>Диапазон дат: с {new Date(dateRange?.[0] || filteredHistory[0].date).toLocaleDateString('ru-RU')}
                        по {new Date(dateRange?.[1] || filteredHistory[filteredHistory.length - 1].date).toLocaleDateString('ru-RU')}</p>
                </div>
            )}
        </div>
    );
};