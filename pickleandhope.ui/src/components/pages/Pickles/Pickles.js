import React, { Component } from 'react';
import SinglePickle from '../../shared/SinglePickle/SinglePickle';
import { getPickles } from '../../../helpers/data/pickleData';

class Pickle extends Component {
    state = {
        pickles: [
            // {
            //     id: 1,
            //     type: "dill",
            //     numberInStock: 12,
            //     size: "large",
            //     price: 5
            // },
            // {
            //     id: 2,
            //     type: "butter",
            //     numberInStock: 12,
            //     size: "large",
            //     price: 5
            // }
        ],
    };

    componentDidMount() {
        getPickles()
        .then(pickles => this.setState({ pickles: pickles }))
    }

    render() {

        const { pickles } = this.state;

        return  pickles.map(pickle => 
            <SinglePickle key={pickle.id} pickle={pickle} />)
    }
}

export default Pickle;