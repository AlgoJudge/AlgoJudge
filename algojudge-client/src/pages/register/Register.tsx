import { useTranslation } from "react-i18next";

function Register() {
    const { t } = useTranslation();

    return (
        <>
            <h1>{t('Register')}</h1>
        </>
    )
}

export default Register;