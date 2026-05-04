import React, { useState } from 'react';
import { Tabs, Tab, Button, TabList, TabPanels, TabPanel } from '@carbon/react';
import { EmployeeCompetenceHeatmap } from './EmployeeCompetenceHeatmap';
import { PositionCompetenceMatrix } from './PositionCompetenceMatrix';
import { OrganizationalMaturityMatrix } from './OrganizationalMaturityMatrix';

export const CompetenceDashboard: React.FC = () => {
    const [selectedTab, setSelectedTab] = useState(0);

    return (
        <div style={{ maxWidth: '1400px', margin: '0 auto', padding: '2rem' }}>
            <h1>Панель управления компетенциями</h1>
            <p style={{ color: '#6f6f6f', marginBottom: '2rem' }}>
                Аналитика и матрицы для администраторов системы
            </p>

            <Tabs selectedIndex={selectedTab} onChange={({ selectedIndex }) => setSelectedTab(selectedIndex)}>
                <TabList>
                    <Tab>Тепловая карта сотрудников</Tab>
                    <Tab>Матрица должностей</Tab>
                    <Tab>Матрица зрелости</Tab>
                    <Tab>Экспорт отчетов</Tab>
                </TabList>
                <TabPanels>
                    <TabPanel>
                        <EmployeeCompetenceHeatmap
                            onEmployeeClick={(id) => console.log('Click', id)}
                        />
                    </TabPanel>
                    <TabPanel>
                        <PositionCompetenceMatrix
                        />
                    </TabPanel>
                    <TabPanel>
                        <OrganizationalMaturityMatrix/>
                    </TabPanel>
                    <TabPanel>
                        <div style={{ padding: '2rem' }}>
                            <h3>Экспорт данных</h3>
                            <div style={{ display: 'flex', gap: '1rem', marginTop: '1rem' }}>
                                <Button>Экспорт в Excel</Button>
                                <Button>Экспорт в PDF</Button>
                                <Button>Сформировать отчет</Button>
                            </div>
                        </div>
                    </TabPanel>
                </TabPanels>
            </Tabs>
        </div>
    );
};