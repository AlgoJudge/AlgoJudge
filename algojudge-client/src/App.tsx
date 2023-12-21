import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [ping, setPing] = useState<string>();

    useEffect(() => {
        fetchPing();
    }, []);

    const contents = ping === undefined
        ? <p>Loading...</p>
        : <p>{ping}</p>;

    return (
        <div>
            <h1>Ping</h1>
            {contents}
        </div>
    );

    async function fetchPing() {
        const response = await fetch('ping/ping');
        const data = await response.text();
        setPing(data);
    }
}

export default App;