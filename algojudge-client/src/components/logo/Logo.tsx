import image from '../../assets/algojudge2.png'

function Logo(props: { size: string | number | undefined; }) {
    return (
        <>
            <img src={image} height={props.size}></img>
        </>
    )
}

export default Logo;