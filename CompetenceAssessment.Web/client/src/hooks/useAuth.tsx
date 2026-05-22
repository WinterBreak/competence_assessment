// src/hooks/useAuth.ts
import { useState, useEffect } from 'react';

export const useAuth = () => {
    const [roles, setRoles] = useState<string[]>([]);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const storedRoles = localStorage.getItem('roles');
        const userId = localStorage.getItem('userId');

        if (storedRoles && userId) {
            try {
                const parsedRoles = JSON.parse(storedRoles);
                setRoles(Array.isArray(parsedRoles) ? parsedRoles : []);
                setIsAuthenticated(true);
            } catch {
                setRoles([]);
                setIsAuthenticated(false);
            }
        }

        setIsLoading(false);
    }, []);

    const hasRole = (role: string): boolean => {
        return roles.includes(role);
    };

    const hasAnyRole = (roleList: string[]): boolean => {
        return roleList.some(role => roles.includes(role));
    };

    const hasAllRoles = (roleList: string[]): boolean => {
        return roleList.every(role => roles.includes(role));
    };

    const logout = () => {
        localStorage.removeItem('userId');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('roles');
        setIsAuthenticated(false);
        setRoles([]);
    };

    return {
        roles,
        isAuthenticated,
        isLoading,
        hasRole,
        hasAnyRole,
        hasAllRoles,
        logout
    };
};