import {
    TextInput,
    PasswordInput,
    Checkbox,
    Anchor,
    Paper,
    Title,
    Text,
    Container,
    Group,
    Button,
} from '@mantine/core';
import classes from './Login.module.css'
import { useTranslation } from "react-i18next";
import { loginApi } from '../../../api/Api'
import { useState } from 'react';
import { Navigate } from 'react-router-dom';

function Login() {
    const { t } = useTranslation();

    const [ logged, setLogged ] = useState(false);

    const MakeLogin = async () => {
        await loginApi.Login("test@test.pl", "Test1!");
        setLogged(true);
    }

    return (
        <Container size={420} my={40}>
            {logged && (<Navigate to="/manage" replace={true} />)}
            <Title ta="center" className={classes.title}>
                {t('Login')}
            </Title>
            <Text c="dimmed" size="sm" ta="center" mt={5}>
                Do not have an account yet?{' '}
                <Anchor size="sm" component="button">
                    Create account
                </Anchor>
            </Text>

            <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                <TextInput label="Email" placeholder="you@mantine.dev" required />
                <PasswordInput label="Password" placeholder="Your password" required mt="md" />
                <Group justify="space-between" mt="lg">
                    <Checkbox label="Remember me" />
                    <Anchor component="button" size="sm">
                        Forgot password?
                    </Anchor>
                </Group>
                <Button fullWidth mt="xl" onClick={MakeLogin}>
                    Sign in
                </Button>
            </Paper>
        </Container>
    );
}

export default Login;