import React, { Component } from 'react';
import ImageCard from './ImageCard'

class Images extends Component {
    constructor(props) {
        super(props)
    }

    render() {
        const imageCards = this.props.cards.map(image => <div key={image.id} className="p-2 col-12 col-md-3">
            <ImageCard
                key={image.id}
                id={image.id}
                name={image.name}
                description={image.description}
                picture={image.picture}
                dateCreated={image.dateCreated}
                deleteImage={this.props.deleteImage}/>
            </div>)

        return (
            <div className="row p-2">
                { imageCards}
            </div>
            )

    }
}



export default Images;