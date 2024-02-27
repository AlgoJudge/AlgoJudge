import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

function Home() {
    const { t } = useTranslation();

    return (
        <>
            <h1>{t('Home')}</h1>
            <Link to="/manage">{t('Manage activities')}</Link>
        </>
    )
}

export default Home;