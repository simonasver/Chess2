import { useContext, useState } from 'react';
import GameContext from '../../store/game-context';

import classes from './Tile.module.css';

const Tile = (props) => {
    const [isPressed, setIsPressed] = useState(false);

    const gameCtx = useContext(GameContext);

    let colClass;

    if(gameCtx.color === 0){
        colClass = classes.red;
    }

    if(gameCtx.color === 2){
        colClass = classes.green;
    }

    if(gameCtx.color === 1){
        colClass = classes.blue;
    }

    if(gameCtx.color === 3){
        colClass = classes.yellow;
    }
    
    const onClickHandler = () => {
        setIsPressed(prevState => !prevState);

        console.log('X: ' + props.node + ' Y: ' + props.row);
    };

    return (
        <div onClick={onClickHandler} className={`${classes.tile} ${isPressed ? colClass : ''} ${props.obstacle ? classes.obstacle : ''} `}></div>
    );
};

export default Tile;