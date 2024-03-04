import { Burger, Center, Container, Group, Menu, useMantineColorScheme } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconChevronDown } from '@tabler/icons-react';
import { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { EventType } from '../../../api/EventDispatcher';
import { useAuth } from '../../provider/AuthProvider';
import { useSession } from '../../provider/SessionProvider';
import Logo from '../logo/Logo';
import classes from './Header.module.css';
import { notifications } from '@mantine/notifications';
import { usePreferences } from '../../provider/PreferencesProvider';

function Header() {
    const [opened, { toggle }] = useDisclosure(false);
    const { eventDispatcher } = useSession();

    const notify = (msg: string) => {
        notifications.show({
            title: 'Connection error',
            message: 'Server response ' + msg,
            color: "red"
        })
    }

    useEffect(() => {
        // TODO
        eventDispatcher.addEventListener(EventType.FORBIDDEN, () => notify('FORBIDDEN'));
        eventDispatcher.addEventListener(EventType.UNAUTHORIZED, () => notify('UNAUTHORIZED'));
        eventDispatcher.addEventListener(EventType.INVALID_STATUS_CODE, () => notify('INVALID_STATUS_CODE'));
    }, []);

    const { t, i18n } = useTranslation();
    const { user, logout } = useAuth();



    const { theme, lang } = usePreferences();
    const { setColorScheme } = useMantineColorScheme();

    useEffect(() => { if (theme) setColorScheme(theme) }, [theme])
    useEffect(() => { if (lang) i18n.changeLanguage(lang) }, [lang])

    const links = user === undefined ? [] : user ?[
        { link: '/', label: t('Home') },
        {
            link: '#1',
            label: user.email,
            links: [
                { link: '#logout', label: 'Logout', func: () => logout() },
            ],
        }
    ] : [
        { link: '/', label: t('Home') },
        { link: '/login', label: t('Login') },
        { link: '/register', label: t('Register') }
    ];

    const items = links.map((link) => {
        const menuItems = link.links?.map((item) => (
            <Menu.Item key={item.link} onClick={(event) => { event.preventDefault(); if (item.func) item.func(); }}>{item.label}</Menu.Item>
        ));

        if (menuItems) {
            return (
                <Menu key={link.label} trigger="hover" transitionProps={{ exitDuration: 0 }} withinPortal>
                    <Menu.Target>
                        <a
                            href={link.link}
                            className={classes.link}
                            onClick={(event) => event.preventDefault()}
                        >
                            <Center>
                                <span className={classes.linkLabel}>{link.label}</span>
                                <IconChevronDown size="0.9rem" stroke={1.5} />
                            </Center>
                        </a>
                    </Menu.Target>
                    <Menu.Dropdown>{menuItems}</Menu.Dropdown>
                </Menu>
            );
        }

        return (
            <Link to={link.link} className={classes.link}>
                {link.label}
            </Link>
        );
    });

    return (
        <header className={classes.header}>
            <Container size="md">
                <div className={classes.inner}>
                    <Link to="/"><Logo size={28} /></Link>
                    <Group gap={5} visibleFrom="sm">
                        {items}
                    </Group>
                    <Burger opened={opened} onClick={toggle} size="sm" hiddenFrom="sm" />
                </div>
            </Container>
        </header>
    );
}

export default Header;