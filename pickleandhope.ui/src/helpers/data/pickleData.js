import axios from 'axios';

const getPickles = () => new Promise((resolve, reject) => {
    axios.get("https://localhost:44352/api/pickles")
    .then((result) => resolve(result.data))
    .catch(error => reject(error))
})

export {getPickles};