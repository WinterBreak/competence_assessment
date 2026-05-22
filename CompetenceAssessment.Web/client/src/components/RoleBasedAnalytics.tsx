import React, { useEffect, useState } from 'react';
import { CompetenceDashboard } from "../pages/assessment/components/analytics/CompetenceDashboard";
import { CompetenceDevelopmentTracker } from "../pages/assessment/components/analytics/CompetenceDevelopmentTracker";
import { Loading } from '@carbon/react';

export const RoleBasedAnalytics: React.FC = () => {
    const [isAdmin, setIsAdmin] = useState<boolean | null>(null);

    useEffect(() => {
        const roles = localStorage.getItem('roles');
        if (roles) {
            try {
                const parsedRoles = JSON.parse(roles);
                setIsAdmin(parsedRoles.includes('Администратор'));
            } catch {
                setIsAdmin(false);
            }
        } else {
            setIsAdmin(false);
        }
    }, []);

    if (isAdmin === null) {
        return <Loading description="Загрузка..." withOverlay={false} />;
    }

    if (isAdmin) {
        return <CompetenceDashboard />;
    } else {
        return <CompetenceDevelopmentTracker />;
    }
};