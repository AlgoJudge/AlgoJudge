import {
    Alert,
    Anchor,
    Box,
    Button,
    Container,
    LoadingOverlay,
    Paper,
    PasswordInput,
    Text,
    TextInput,
    Title
} from '@mantine/core';
import { IconInfoCircle } from '@tabler/icons-react';
import { useState } from 'react';
import { useTranslation } from "react-i18next";
import { Navigate } from 'react-router-dom';
import { UnauthorizedError } from '../../../api/ApiRequester';
import { useAuth } from '../../provider/AuthProvider';
import { useSession } from '../../provider/SessionProvider';
import classes from './Login.module.css';

function Login() {
    const { t } = useTranslation();

    const [logged, setLogged] = useState(false);
    const [email, setEmail] = useState('test@test.pl');
    const [password, setPassword] = useState('Test1!');
    const [error, setError] = useState<string | undefined>(undefined);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const { login } = useAuth();
    const { loginApi } = useSession();
    const icon = <IconInfoCircle />;

    const makeLogin = async () => {
        setError(undefined);
        setIsLoading(true);
        try {
            await loginApi.login(email, password);
        } catch (e) {
            if (e instanceof UnauthorizedError) {
                setError(t('Invalid email or password'));
            } else {
                setError(t('Error'));
            }
            setIsLoading(false);
            return;
        }
        setLogged(true);
        login({ email, password });
    }

    return (
        <Container size={420} my={40}>
            {logged && (<Navigate to="/manage" replace={true} />)}
            <Title ta="center" className={classes.title}>
                {t('Login')}
            </Title>
            <Text c="dimmed" size="sm" ta="center" mt={5}>
                {t('Do not have an account yet?')}{' '}
                <Anchor size="sm" component="button">
                    {t('Create account')}
                </Anchor>
            </Text>

            {error && (<Alert variant="filled" color="red" withCloseButton title={t('Error')} icon={icon} my="2rem">{error}</Alert>)}

            <Box pos="relative">
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                    <LoadingOverlay visible={isLoading} zIndex={1000} overlayProps={{ radius: "sm", blur: 2 }} />
                    <TextInput label={t('Email')} placeholder={t('mail@example.com')} required onChange={(e) => setEmail(e.target.value)} />
                    <PasswordInput label={t('Password')} placeholder={t('Your password')} required mt="md" onChange={(e) => setPassword(e.target.value)} />
                    {/*<Group justify="space-between" mt="lg">*/}
                    {/*    <Checkbox label="Remember me" />*/}
                    {/*    <Anchor component="button" size="sm">*/}
                    {/*        Forgot password?*/}
                    {/*    </Anchor>*/}
                    {/*</Group>*/}
                    <Button fullWidth mt="xl" onClick={makeLogin}>
                        {t('Sign in')}
                    </Button>
                </Paper>
            </Box>
        </Container>
    );
}

export default Login;