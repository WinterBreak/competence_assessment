import React from 'react';
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
    return (<Theme theme="g10">
        <HeaderContainer render={({isSideNavExpanded, onClickSideNavExpand}) => <>
            <Header aria-label="Оценка компетенций">
                <SkipToContent />
                <HeaderMenuButton aria-label={isSideNavExpanded ? 'Close menu' : 'Open menu'} 
                                  onClick={onClickSideNavExpand} 
                                  isActive={isSideNavExpanded} 
                                  aria-expanded={isSideNavExpanded}
                                  style={{ display: 'flex' }}/>
                <HeaderName href="#">
                    Оценка компетенций
                </HeaderName>
                <HeaderNavigation aria-label="Оценка компетенций">
                    <HeaderMenuItem href="/templates">Шаблоны</HeaderMenuItem>
                    <HeaderMenuItem href="/competence_models">Модели компетенций</HeaderMenuItem>
                    <HeaderMenuItem href="/tasks">Задания</HeaderMenuItem>
                    <HeaderMenuItem href="/competencies">Компетенции</HeaderMenuItem>
                    <HeaderMenuItem href="/users">Пользователи</HeaderMenuItem>
                </HeaderNavigation>
                <SideNav aria-label="Side navigation"
                    expanded={isSideNavExpanded}
                    onOverlayClick={onClickSideNavExpand}
                    isPersistent={false} >
                    <SideNavItems>
                        <SideNavLink href="">
                            Оценка
                        </SideNavLink>
                        <SideNavLink href="">
                            Администрирование
                        </SideNavLink>
                    </SideNavItems>
                </SideNav>
            </Header>
        </>} />
    </Theme>);
}

export default Navbar;