import React, { Component } from 'react'
import TagBadge from './TagBadge'


class TagGroup extends Component{
    render() {
        const TagBadges = this.props.tags.map(tag => <TagBadge key={tag.tagId} Tag={tag} handleRemoveTag={this.props.handleRemoveTag} deleteButton={this.props.deleteButton}/>);
        if (TagBadges.length > 0){
            return TagBadges;
        }
        return null;
    }

}


export default TagGroup