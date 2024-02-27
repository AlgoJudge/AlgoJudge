import { Container, Group, Anchor, Affix } from '@mantine/core';
import classes from './Footer.module.css';
import Logo from '../logo/Logo';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

function Footer() {
    const { t } = useTranslation();

    const links = [
        { link: '#', label: t('About') },
    ];

    const items = links.map((link) => (
        <Anchor<'a'>
            c="dimmed"
            key={link.label}
            href={link.link}
            onClick={(event) => event.preventDefault()}
            size="sm"
        >
            {link.label}
        </Anchor>
    ));

    return (
        <Affix withinPortal={false} position={{ bottom: 0, left: 0, right: 0 }}>
            <div className={classes.footer}>
                <Container className={classes.inner}>
                    <Link to="/"><Logo size={28} /></Link>
                    <Group className={classes.links}>{items}</Group>
                </Container>
            </div>
        </Affix>
    );
}

export default Footer;