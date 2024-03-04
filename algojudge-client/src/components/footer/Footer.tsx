import { Affix, Anchor, Center, Container, Group, Menu } from '@mantine/core';
import { IconChevronUp } from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import Logo from '../logo/Logo';
import classes from './Footer.module.css';
import { usePreferences } from '../../provider/PreferencesProvider';
import { useEffect } from 'react';

function Footer() {
    const { t } = useTranslation();
    const { theme, setTheme, setLang } = usePreferences();

    useEffect(() => console.log("XXD", [theme]))

    const links = [
        { link: 'https://algojudge.pl', label: t('About'), prev: false },
    ];

    const links2 = [
        {
            link: '#1',
            label: 'Lang',
            links: [
                { link: '#1-en', label: 'English', func: () => setLang('en') },
                { link: '#1-pl', label: 'Polski', func: () => setLang('pl') },
            ],
        },
        {
            link: '#2',
            label: 'Theme',
            links: [
                { link: '#2-light', label: 'Light', func: () => setTheme('light') },
                { link: '#2-dark', label: 'Dark', func: () => { console.log("DARK"); setTheme('dark') } },
            ],
        }
    ];

    const items = links.map((link) => (
        <Anchor<'a'>
            c="dimmed"
            key={link.label}
            href={link.link}
            onClick={(event) => link.prev ? event.preventDefault() : {}}
            target="_blank"
            size="sm"
        >
            {link.label}
        </Anchor>
    ));
    const items2 = links2.map((link) => {
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
                                <IconChevronUp size="0.9rem" stroke={1.5} />
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
    const items3 = [...items, ...items2];

    return (
        <Affix withinPortal={false} position={{ bottom: 0, left: 0, right: 0 }}>
            <div className={classes.footer}>
                <Container className={classes.inner}>
                    <Link to="/"><Logo size={28} /></Link>
                    <Group className={classes.links}>{items3}</Group>
                </Container>
            </div>
        </Affix>
    );
}

export default Footer;