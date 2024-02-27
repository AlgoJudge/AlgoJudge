import { useEffect, useState } from "react";
import { activityApi } from '../../../api/Api'
import { Table } from '@mantine/core';
import { ActivityInfo } from '../../../api/ActivityApi';

function Manage() {

    const [activities, setActivities] = useState<ActivityInfo[]>([]);

    useEffect(() => {
        activityApi.List().then((res) => setActivities(res));
    }, [])

    return (
        <>
            <h1>Manage</h1>
            <Table>
                <Table.Thead>
                    <Table.Tr>
                        <Table.Th>Id</Table.Th>
                        <Table.Th>Short</Table.Th>
                        <Table.Th>Name</Table.Th>
                        <Table.Th>Start date</Table.Th>
                        <Table.Th>End date</Table.Th>
                    </Table.Tr>
                </Table.Thead>
                <Table.Tbody>
                    {activities.map((element) =>
                        <Table.Tr key={element.id}>
                            <Table.Td>{element.id}</Table.Td>
                            <Table.Td>{element.shortName}</Table.Td>
                            <Table.Td>{element.name}</Table.Td>
                            <Table.Td>{element.startDate}</Table.Td>
                            <Table.Td>{element.endDate}</Table.Td>
                        </Table.Tr>)
                    }
                </Table.Tbody>
            </Table>
        </>
    )
}

export default Manage;