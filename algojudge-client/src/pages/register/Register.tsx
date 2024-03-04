import { Alert, Anchor, Box, Button, Checkbox, Container, Group, LoadingOverlay, Paper, PasswordInput, Text, TextInput, Title } from "@mantine/core";
import { useTranslation } from "react-i18next";
import classes from './Register.module.css';
import { IconInfoCircle } from "@tabler/icons-react";
import { useState } from "react";
import { useSession } from "../../provider/SessionProvider";
import { UnauthorizedError } from "../../../api/ApiRequester";

function Register() {
    const { t } = useTranslation();
    const [error, setError] = useState<string | undefined>(undefined);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [email, setEmail] = useState('test@test.pl');
    const [password, setPassword] = useState('Test1!');
    const { loginApi } = useSession();
    const icon = <IconInfoCircle />;

    const makeRegister = async () => {
        setError(undefined);
        setIsLoading(true);
        try {
            await loginApi.register(email, password);
        } catch (e) {
            if (e instanceof UnauthorizedError) {
                setError(t('Invalid email or password'));
            } else {
                setError(t('Error'));
            }
            setIsLoading(false);
            return;
        }
        //setLogged(true);
        //login({ email, password });
    }

    return (
        <Container size={420} my={40}>
            <Title ta="center" className={classes.title}>
                {t('Register')}
            </Title>

            {error && (<Alert variant="filled" color="red" withCloseButton title={t('Error')} icon={icon} my="2rem">{error}</Alert>)}

            <Box pos="relative">
                <Paper withBorder shadow="md" p={30} mt={30} radius="md">
                    <LoadingOverlay visible={isLoading} zIndex={1000} overlayProps={{ radius: "sm", blur: 2 }} />
                    <TextInput label={t('Email')} placeholder={t('mail@example.com')} required onChange={(e) => setEmail(e.target.value)} />
                    <PasswordInput label={t('Password')} placeholder={t('Your password')} required mt="md" onChange={(e) => setPassword(e.target.value)} />
                    <Group justify="space-between" mt="lg">
                        <Checkbox label={t('I accept the terms and conditions')} />
                        <Anchor component="button" size="sm">
                            {t('I accept the terms and conditions')}
                        </Anchor>
                    </Group>
                    <Button fullWidth mt="xl" onClick={makeRegister}>
                        {t('Sign up')}
                    </Button>
                </Paper>
            </Box>
        </Container>
    )
}

export default Register;