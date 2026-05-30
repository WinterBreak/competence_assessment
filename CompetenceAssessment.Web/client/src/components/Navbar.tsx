import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import {
    HeaderNavigation,
    HeaderMenuItem,
    Header,
    HeaderContainer,
    HeaderMenuButton,
    HeaderName,
    SideNav,
    SideNavItems,
    SkipToContent,
    Theme,
    SideNavLink, Loading,
} from '@carbon/react';
import { Logout } from '@carbon/react/icons';
import {authService} from "../pages/auth/services/authService";
import {useAuth} from "../hooks/useAuth";
// TODO разграничение отображения по ролям (мб просто два разных компонента сделать
function Navbar() {
    const { hasRole, isLoading } = useAuth();
    const location = useLocation();
    const navigate = useNavigate();
    const [activeSection, setActiveSection] = useState<string>('assessment');

    const canAccessAdmin = (() => {
        const roles = localStorage.getItem('roles');
        if (!roles) return false;
        try {
            return JSON.parse(roles).includes('Администратор');
        } catch {
            return false;
        }
    })();
    
    useEffect(() => {
        if (location.pathname.startsWith('/admin')) {
            if (!canAccessAdmin) {
                navigate('/');
                return;
            }
            setActiveSection('admin');
        } else {
            setActiveSection('assessment');
        }
    }, [location.pathname, canAccessAdmin, navigate]);

    if (isLoading) {
        return <Loading description="Проверка доступа..." withOverlay={false} />;
    }
    
    const navigationLinks : Record<string, { label: string; href: string }[]> = {
        admin: [
            { label: 'Шаблоны', href: '/admin/templates' },
            { label: 'Модели компетенций', href: '/admin/competence_models' },
            { label: 'Задания', href: '/admin/tasks' },
            { label: 'Компетенции', href: '/admin/competencies' },
            { label: 'Пользователи', href: '/admin/users' },
        ],
        assessment: canAccessAdmin
            ? [{ label: 'Оценка', href: '/' }, { label: 'Аналитика', href: '/analytics' }]
            : [{ label: 'Оценка', href: '/' }, { label: 'Аналитика', href: '/analytics' }]
    }

    const handleSideNavClick = (section: string, defaultPath: string) => {
        setActiveSection(section);
        navigate(defaultPath);
    };

    const handleLogout = async () => {
        if (window.confirm('Вы уверены, что хотите выйти из системы?')) {
            localStorage.removeItem('userId');
            localStorage.removeItem('userEmail');
            await authService.logout();
            navigate('/login');
        }
    };

    return (<Theme theme="g10">
        <HeaderContainer render={({isSideNavExpanded, onClickSideNavExpand}) => <>
            <Header aria-label="Оценка компетенций">
                <SkipToContent />
                <HeaderMenuButton aria-label={isSideNavExpanded ? 'Закрыть меню' : 'Открыть меню'}
                                  onClick={onClickSideNavExpand}
                                  isActive={isSideNavExpanded}
                                  aria-expanded={isSideNavExpanded}
                                  style={{ display: 'flex' }}/>
                <HeaderName href="" prefix="">
                    Оценка компетенций
                </HeaderName>
                <HeaderNavigation aria-label="Навигация">
                    {navigationLinks[activeSection].map((link, index) => (
                        <HeaderMenuItem key={index} href={link.href}>
                            {link.label}
                        </HeaderMenuItem>
                    ))}
                </HeaderNavigation>

                <SideNav aria-label="Side navigation"
                         expanded={isSideNavExpanded}
                         onOverlayClick={onClickSideNavExpand}
                         isPersistent={false}>
                    <SideNavItems>
                        <SideNavLink  onClick={() => {
                            handleSideNavClick('assessment', '/');
                            onClickSideNavExpand();}}
                                      isActive={activeSection === 'assessment'}>
                            Оценка
                        </SideNavLink>

                        {canAccessAdmin && (
                        <SideNavLink onClick={() => {
                            handleSideNavClick('admin', 'admin/templates');
                            onClickSideNavExpand();}}
                                     isActive={activeSection === 'admin'}>
                            Администрирование
                        </SideNavLink>)}
                    </SideNavItems>
                    
                    <div style={{
                        position: 'absolute',
                        bottom: 0,
                        left: 0,
                        right: 0,
                        padding: '1rem',
                        borderTop: '1px solid #e0e0e0',
                        marginTop: 'auto'
                    }}>
                        <SideNavLink
                            onClick={() => {
                                handleLogout();
                                onClickSideNavExpand();
                            }}
                            renderIcon={Logout}
                            style={{ cursor: 'pointer' }}
                        >
                            Выйти
                        </SideNavLink>
                    </div>
                </SideNav>
            </Header>
        </>} />
    </Theme>);
}

export default Navbar;