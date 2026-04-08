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
    SideNavLink,
} from '@carbon/react';
// TODO разграничение отображения по ролям (мб просто два разных компонента сделать
function Navbar() {
    const location = useLocation();
    const navigate = useNavigate();
    const [activeSection, setActiveSection] = useState<string>('admin');

    useEffect(() => {
        if (location.pathname.startsWith('/admin')) {
            setActiveSection('admin');
        } else {
            setActiveSection('assessment');
        }
    }, [location.pathname]);

    const navigationLinks : Record<string, { label: string; href: string }[]> = {
        admin: [
            { label: 'Шаблоны', href: '/admin/templates' },
            { label: 'Модели компетенций', href: '/admin/competence_models' },
            { label: 'Задания', href: '/admin/tasks' },
            { label: 'Компетенции', href: '/admin/competencies' },
            { label: 'Пользователи', href: '/admin/users' },
        ],
        assessment: [
            { label: 'Оценка', href: '/' },
            { label: 'История', href: '/assessment_history' },
        ]
    };

    const handleSideNavClick = (section: string, defaultPath: string) => {
        setActiveSection(section);
        navigate(defaultPath);
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
                <HeaderName href="#">
                    Оценка компетенций
                </HeaderName>
                {/*<HeaderNavigation aria-label="Оценка компетенций">*/}
                {/*    <HeaderMenuItem href="/templates">Шаблоны</HeaderMenuItem>*/}
                {/*    <HeaderMenuItem href="/competence_models">Модели компетенций</HeaderMenuItem>*/}
                {/*    <HeaderMenuItem href="/tasks">Задания</HeaderMenuItem>*/}
                {/*    <HeaderMenuItem href="/competencies">Компетенции</HeaderMenuItem>*/}
                {/*    <HeaderMenuItem href="/users">Пользователи</HeaderMenuItem>*/}
                {/*</HeaderNavigation>*/}

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
                    isPersistent={false} >
                    <SideNavItems>
                        <SideNavLink  onClick={() => {
                            handleSideNavClick('assessment', '/');
                            onClickSideNavExpand();}}
                                      isActive={activeSection === 'assessment'}>
                            Оценка
                        </SideNavLink>
                        <SideNavLink onClick={() => { 
                            handleSideNavClick('admin', 'admin/templates');
                            onClickSideNavExpand();}}
                                     isActive={activeSection === 'admin'}>
                            Администрирование
                        </SideNavLink>
                    </SideNavItems>
                </SideNav>
            </Header>
        </>} />
    </Theme>);
}

export default Navbar;