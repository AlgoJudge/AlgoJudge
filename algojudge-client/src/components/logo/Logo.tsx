import image from '../../assets/algojudge2.png'
import classes from './Logo.module.css'

function Logo(props: { size: string | number | undefined; }) {
    return (
        <>
            <img className={classes.ajlogo} src={image} height={props.size}></img>
        </>
    )
}

export default Logo;